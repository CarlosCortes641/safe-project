using SafeProject.Web.Demo;

namespace SafeProject.Web.ViewModels;

public sealed class StartProjectRequest
{
    public string Kind { get; set; } = "water";
    public string ProductId { get; set; } = "";
    public string Zip { get; set; } = "28202";
    public PaymentPath PaymentPath { get; set; } = PaymentPath.CashSandbox;
}

public sealed class ScheduleRequest
{
    public DateOnly Date { get; set; }
}

public sealed class RealtorFormRequest
{
    public string Role { get; set; } = "realtor";
    public string Stage { get; set; } = "inspection";
    public string SupportNeed { get; set; } = "trade";
    public string FundingReview { get; set; } = "closing_review";
    public string Contact { get; set; } = "";
    public string? PreferredDate { get; set; }
    public IFormFile? File { get; set; }
}

public sealed class OpsDashboardViewModel
{
    public required IReadOnlyList<ProjectRecord> Projects { get; init; }
    public required IReadOnlyList<RealtorRequest> RealtorRequests { get; init; }
    public required IReadOnlyList<AuditEvent> Audit { get; init; }
    public required IReadOnlyList<DayCapacity> Days { get; init; }
    public required int CapacityPerDay { get; init; }
}
