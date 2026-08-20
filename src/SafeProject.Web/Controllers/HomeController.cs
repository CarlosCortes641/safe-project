using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SafeProject.Web.Catalog;
using SafeProject.Web.Demo;
using SafeProject.Web.Models;
using SafeProject.Web.ViewModels;

namespace SafeProject.Web.Controllers;

public class HomeController : Controller
{
    private readonly CatalogStore _catalog;
    private readonly DemoStore _demo;

    public HomeController(CatalogStore catalog, DemoStore demo)
    {
        _catalog = catalog;
        _demo = demo;
    }

    public IActionResult Index()
    {
        var model = new HomeViewModel
        {
            WaterHeaters = _catalog.WaterHeaters,
            Hvac = _catalog.Hvac,
            ScheduleDays = _demo.InstallationDays()
                .Select(d => new ScheduleDay(d.Date, d.IsFull))
                .ToList(),
            CapacityPerDay = _demo.CapacityPerDay,
            SynchronyUrl = CatalogStore.SynchronyUrl,
            PhoneDisplay = CatalogStore.PhoneDisplay,
            PhoneTel = CatalogStore.PhoneTel,
            Email = CatalogStore.Email
        };

        return View(model);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
