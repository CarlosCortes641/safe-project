using Microsoft.AspNetCore.Mvc;
using SafeProject.Web.Catalog;
using SafeProject.Web.ViewModels;

namespace SafeProject.Web.Controllers;

public class WaterHeatersController : Controller
{
    private readonly CatalogStore _catalog;

    public WaterHeatersController(CatalogStore catalog)
    {
        _catalog = catalog;
    }

    [HttpGet("/water-heaters")]
    public IActionResult Index()
    {
        return View(new WaterHeatersViewModel
        {
            WaterHeaters = _catalog.WaterHeaters
        });
    }
}
