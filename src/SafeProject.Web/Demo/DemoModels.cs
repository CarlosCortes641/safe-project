namespace SafeProject.Web.Demo;

public enum PaymentPath
{
    CashSandbox,
    SynchronySandbox,
    Safe24Sandbox
}

public enum ProjectStatus
{
    Draft,
    ScopeApproved,
    PaymentPending,
    PaymentAuthorizedSandbox,
    Scheduled,
    Cancelled
}

public sealed class ProjectRecord
{
    public required string Id { get; init; }
    public required string Kind { get; init; }
    public required string ProductId { get; init; }
    public required string ProductNameEn { get; init; }
    public required string ProductNameEs { get; init; }
    public required int PriceUsd { get; init; }
    public string Zip { get; set; } = "";
    public string PropertyType { get; set; } = "single";
    public string InspectPath { get; set; } = "remote";
    public bool ScopeAccepted { get; set; }
    public PaymentPath PaymentPath { get; set; }
    public ProjectStatus Status { get; set; } = ProjectStatus.Draft;
    public DateOnly? InstallDate { get; set; }
    public DateTime CreatedUtc { get; init; } = DateTime.UtcNow;
    public DateTime UpdatedUtc { get; set; } = DateTime.UtcNow;
}

public sealed class RealtorRequest
{
    public required string Id { get; init; }
    public required string Role { get; init; }
    public required string Stage { get; init; }
    public required string SupportNeed { get; init; }
    public required string FundingReview { get; init; }
    public string Contact { get; init; } = "";
    public string FileName { get; init; } = "";
    public DateOnly? PreferredDate { get; init; }
    public DateTime CreatedUtc { get; init; } = DateTime.UtcNow;
}

public sealed class AuditEvent
{
    public required string Id { get; init; }
    public required string EntityType { get; init; }
    public required string EntityId { get; init; }
    public required string Action { get; init; }
    public string Detail { get; init; } = "";
    public DateTime AtUtc { get; init; } = DateTime.UtcNow;
}

public sealed class DayCapacity
{
    public required DateOnly Date { get; init; }
    public required int Used { get; init; }
    public required int Capacity { get; init; }
    public bool IsFull => Used >= Capacity;
    public int Remaining => Math.Max(0, Capacity - Used);
}
