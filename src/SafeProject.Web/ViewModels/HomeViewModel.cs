using SafeProject.Web.Catalog;

namespace SafeProject.Web.ViewModels;

public sealed class HomeViewModel
{
    public required IReadOnlyList<WaterHeaterProduct> WaterHeaters { get; init; }
    public required IReadOnlyList<HvacProduct> Hvac { get; init; }
    public required IReadOnlyList<ScheduleDay> ScheduleDays { get; init; }
    public required int CapacityPerDay { get; init; }
    public required string SynchronyUrl { get; init; }
    public required string PhoneDisplay { get; init; }
    public required string PhoneTel { get; init; }
    public required string Email { get; init; }
}
