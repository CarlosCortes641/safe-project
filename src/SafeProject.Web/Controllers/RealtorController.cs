using Microsoft.AspNetCore.Mvc;
using SafeProject.Web.Demo;
using SafeProject.Web.ViewModels;

namespace SafeProject.Web.Controllers;

public class RealtorController : Controller
{
    private readonly DemoStore _demo;

    public RealtorController(DemoStore demo)
    {
        _demo = demo;
    }

    [HttpGet("/realtor")]
    public IActionResult Index()
    {
        ViewBag.Days = _demo.InstallationDays();
        return View(new RealtorFormRequest());
    }

    [HttpPost("/realtor")]
    [ValidateAntiForgeryToken]
    public IActionResult Create(RealtorFormRequest request)
    {
        var fileName = request.File?.FileName ?? "";
        DateOnly? preferred = DateOnly.TryParse(request.PreferredDate, out var parsed) ? parsed : null;
        var created = _demo.AddRealtorRequest(
            request.Role,
            request.Stage,
            request.SupportNeed,
            request.FundingReview,
            request.Contact,
            fileName,
            preferred);

        TempData["RealtorId"] = created.Id;
        return RedirectToAction(nameof(Received), new { id = created.Id });
    }

    [HttpGet("/realtor/{id}/received")]
    public IActionResult Received(string id)
    {
        var request = _demo.RealtorRequests().FirstOrDefault(r => r.Id == id);
        if (request is null)
        {
            return NotFound();
        }

        return View(request);
    }
}
