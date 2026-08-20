using System.Collections.Concurrent;
using SafeProject.Web.Catalog;

namespace SafeProject.Web.Demo;

public sealed class DemoStore
{
    private readonly object _gate = new();
    private readonly ConcurrentDictionary<string, ProjectRecord> _projects = new();
    private readonly ConcurrentDictionary<string, RealtorRequest> _realtors = new();
    private readonly ConcurrentQueue<AuditEvent> _audit = new();
    private readonly Dictionary<DateOnly, int> _capacity = [];
    private int _projectSeq = 24000;
    private int _realtorSeq = 100;

    public int CapacityPerDay { get; } = CatalogStore.InstallationCapacityPerDay;

    public DemoStore()
    {
        foreach (var day in UpcomingOperatingDays(DateOnly.FromDateTime(DateTime.Now), 8))
        {
            _capacity[day] = 0;
        }

        var days = UpcomingOperatingDays(DateOnly.FromDateTime(DateTime.Now), 5);
        if (days.Count >= 5)
        {
            _capacity[days[0]] = 1;
            _capacity[days[1]] = CapacityPerDay;
            _capacity[days[2]] = 0;
            _capacity[days[3]] = 4;
            _capacity[days[4]] = 0;
        }
        AddAudit("schedule", days[1].ToString("yyyy-MM-dd"), "seed_full_day", $"Seeded {CapacityPerDay}/{CapacityPerDay} for demo");
    }

    public IReadOnlyList<ProjectRecord> Projects() =>
        _projects.Values.OrderByDescending(p => p.UpdatedUtc).ToList();

    public IReadOnlyList<RealtorRequest> RealtorRequests() =>
        _realtors.Values.OrderByDescending(r => r.CreatedUtc).ToList();

    public IReadOnlyList<AuditEvent> Audit() =>
        _audit.Reverse().Take(40).ToList();

    public ProjectRecord? GetProject(string id) =>
        _projects.TryGetValue(id, out var project) ? project : null;

    public IReadOnlyList<DayCapacity> InstallationDays()
    {
        lock (_gate)
        {
            return UpcomingOperatingDays(DateOnly.FromDateTime(DateTime.Now), 5)
                .Select(date => new DayCapacity
                {
                    Date = date,
                    Used = _capacity.GetValueOrDefault(date),
                    Capacity = CapacityPerDay
                })
                .ToList();
        }
    }

    public ProjectRecord StartProject(
        string kind,
        string productId,
        string nameEn,
        string nameEs,
        int priceUsd,
        string zip,
        PaymentPath path)
    {
        var id = $"SPS-{Interlocked.Increment(ref _projectSeq)}";
        var project = new ProjectRecord
        {
            Id = id,
            Kind = kind,
            ProductId = productId,
            ProductNameEn = nameEn,
            ProductNameEs = nameEs,
            PriceUsd = priceUsd,
            Zip = string.IsNullOrWhiteSpace(zip) ? "28202" : zip.Trim(),
            PaymentPath = path,
            Status = ProjectStatus.Draft
        };

        _projects[id] = project;
        AddAudit("project", id, "created", $"{kind} / {productId} / {path}");
        return project;
    }

    public ProjectRecord? Update(string id, Action<ProjectRecord> mutate)
    {
        if (!_projects.TryGetValue(id, out var project))
        {
            return null;
        }

        mutate(project);
        project.UpdatedUtc = DateTime.UtcNow;
        return project;
    }

    public ProjectRecord? ApproveScope(string id)
    {
        if (!_projects.TryGetValue(id, out var project))
        {
            return null;
        }

        project.ScopeAccepted = true;
        project.Status = ProjectStatus.ScopeApproved;
        project.UpdatedUtc = DateTime.UtcNow;
        AddAudit("project", id, "scope_approved", $"{project.Kind} / {project.ProductId}");
        return project;
    }

    public ProjectRecord? MarkPaymentPending(string id)
    {
        if (!_projects.TryGetValue(id, out var project))
        {
            return null;
        }

        project.Status = ProjectStatus.PaymentPending;
        project.UpdatedUtc = DateTime.UtcNow;
        AddAudit("project", id, "payment_pending", project.PaymentPath.ToString());
        return project;
    }

    public ProjectRecord? AuthorizeSandboxPayment(string id)
    {
        if (!_projects.TryGetValue(id, out var project))
        {
            return null;
        }

        project.Status = ProjectStatus.PaymentAuthorizedSandbox;
        project.UpdatedUtc = DateTime.UtcNow;
        AddAudit("payment", id, "sandbox_authorized", $"{project.Kind} {project.PaymentPath} ${project.PriceUsd} posted to internal board (not a live charge)");
        return project;
    }

    public (bool Ok, string Error, ProjectRecord? Project) TrySchedule(string id, DateOnly date)
    {
        if (!_projects.TryGetValue(id, out var project))
        {
            return (false, "not_found", null);
        }

        if (project.Status != ProjectStatus.PaymentAuthorizedSandbox)
        {
            return (false, "payment_required", project);
        }

        lock (_gate)
        {
            var used = _capacity.GetValueOrDefault(date);
            if (used >= CapacityPerDay)
            {
                AddAudit("schedule", id, "rejected_full", date.ToString("yyyy-MM-dd"));
                return (false, "full", project);
            }

            if (project.InstallDate is { } previous)
            {
                _capacity[previous] = Math.Max(0, _capacity.GetValueOrDefault(previous) - 1);
            }

            _capacity[date] = used + 1;
            project.InstallDate = date;
            project.Status = ProjectStatus.Scheduled;
            project.UpdatedUtc = DateTime.UtcNow;
            AddAudit("schedule", id, "booked", $"{date:yyyy-MM-dd} {_capacity[date]}/{CapacityPerDay}");
            return (true, "", project);
        }
    }

    public RealtorRequest AddRealtorRequest(
        string role,
        string stage,
        string supportNeed,
        string fundingReview,
        string contact,
        string fileName,
        DateOnly? preferredDate)
    {
        var id = $"RLT-{Interlocked.Increment(ref _realtorSeq)}";
        var request = new RealtorRequest
        {
            Id = id,
            Role = role,
            Stage = stage,
            SupportNeed = supportNeed,
            FundingReview = fundingReview,
            Contact = contact,
            FileName = fileName,
            PreferredDate = preferredDate
        };
        _realtors[id] = request;
        AddAudit("realtor", id, "requested", $"{role}/{stage}/{fundingReview}");
        return request;
    }

    private void AddAudit(string entityType, string entityId, string action, string detail)
    {
        _audit.Enqueue(new AuditEvent
        {
            Id = Guid.NewGuid().ToString("N")[..8],
            EntityType = entityType,
            EntityId = entityId,
            Action = action,
            Detail = detail
        });
    }

    public static IReadOnlyList<DateOnly> UpcomingOperatingDays(DateOnly start, int count)
    {
        var days = new List<DateOnly>();
        var cursor = start;
        while (days.Count < count)
        {
            cursor = cursor.AddDays(1);
            if (cursor.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
            {
                continue;
            }

            days.Add(cursor);
        }

        return days;
    }
}
