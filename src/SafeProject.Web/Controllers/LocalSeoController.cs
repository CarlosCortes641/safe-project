using Microsoft.AspNetCore.Mvc;
using SafeProject.Web.Infrastructure;

namespace SafeProject.Web.Controllers;

public class LocalSeoController : Controller
{
    [HttpGet("/about")]
    public IActionResult About() => View();

    [HttpGet("/faq")]
    public IActionResult Faq() => View();

    [HttpGet("/areas/{slug}")]
    public IActionResult Area(string slug)
    {
        var city = SeoContent.FindCity(slug);
        if (city is null)
        {
            return NotFound();
        }

        return View(city);
    }

    [HttpGet("/hvac-installation-charlotte")]
    public IActionResult HvacInstallation() => View();

    [HttpGet("/tankless-water-heater-charlotte")]
    public IActionResult Tankless() => View();

    [HttpGet("/guides/water-heater-cost-charlotte")]
    public IActionResult WaterHeaterCostGuide() => View();

    [HttpGet("/guides/hvac-replacement-charlotte")]
    public IActionResult HvacReplacementGuide() => View();
}
