using Microsoft.AspNetCore.Mvc;
using SafeProject.Web.Demo;
using SafeProject.Web.ViewModels;

namespace SafeProject.Web.Controllers;

public class OpsController : Controller
{
    private readonly DemoStore _demo;

    public OpsController(DemoStore demo)
    {
        _demo = demo;
    }

    [HttpGet("/ops")]
    public IActionResult Index()
    {
        var model = new OpsDashboardViewModel
        {
            Projects = _demo.Projects(),
            RealtorRequests = _demo.RealtorRequests(),
            Audit = _demo.Audit(),
            Days = _demo.InstallationDays(),
            CapacityPerDay = _demo.CapacityPerDay
        };

        return View(model);
    }
}
