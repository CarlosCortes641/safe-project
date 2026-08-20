using SafeProject.Web.Demo;

namespace SafeProject.Web.ViewModels;

public sealed class JourneyForm
{
    public string? PropertyType { get; set; }
    public string? InspectPath { get; set; }
    public bool ScopeAccepted { get; set; }
    public PaymentPath? PaymentPath { get; set; }
    public DateOnly? Date { get; set; }
}

public sealed class JourneyViewModel
{
    public required ProjectRecord Project { get; init; }
    public required int Step { get; init; }
    public required IReadOnlyList<DayCapacity> Days { get; init; }
    public string? Error { get; init; }
}
