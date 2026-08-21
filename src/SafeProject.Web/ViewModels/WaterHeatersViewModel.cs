using SafeProject.Web.Catalog;

namespace SafeProject.Web.ViewModels;

public sealed class WaterHeatersViewModel
{
    public required IReadOnlyList<WaterHeaterProduct> WaterHeaters { get; init; }
}
