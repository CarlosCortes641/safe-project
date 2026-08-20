using Microsoft.AspNetCore.Mvc;
using SafeProject.Web.Catalog;
using SafeProject.Web.Demo;
using SafeProject.Web.ViewModels;

namespace SafeProject.Web.Controllers;

public class ProjectController : Controller
{
    private readonly DemoStore _demo;
    private readonly CatalogStore _catalog;

    public ProjectController(DemoStore demo, CatalogStore catalog)
    {
        _demo = demo;
        _catalog = catalog;
    }

    [HttpPost("/project/start")]
    [ValidateAntiForgeryToken]
    public IActionResult Start(StartProjectRequest request)
    {
        ProjectRecord created;
        if (request.Kind == "hvac")
        {
            var hvac = _catalog.Hvac.FirstOrDefault(p => p.Id == request.ProductId) ?? _catalog.Hvac[0];
            created = _demo.StartProject("hvac", hvac.Id, hvac.NameEn, hvac.NameEs, hvac.StartingPriceUsd, request.Zip, request.PaymentPath);
        }
        else
        {
            var product = _catalog.WaterHeaters.FirstOrDefault(p => p.Id == request.ProductId) ?? _catalog.WaterHeaters[0];
            created = _demo.StartProject("water", product.Id, product.NameEn, product.NameEs, product.PriceUsd, request.Zip, request.PaymentPath);
        }

        return RedirectToAction(nameof(Step), new { id = created.Id, step = 0 });
    }

    [HttpGet("/project/{id}")]
    public IActionResult Details(string id) => RedirectToAction(nameof(Step), new { id, step = 0 });

    [HttpGet("/project/{id}/pay")]
    public IActionResult Pay(string id) => RedirectToAction(nameof(Step), new { id, step = 3 });

    [HttpGet("/project/{id}/schedule")]
    public IActionResult Schedule(string id) => RedirectToAction(nameof(Step), new { id, step = 5 });

    [HttpGet("/project/{id}/confirm")]
    public IActionResult Confirm(string id) => RedirectToAction(nameof(Step), new { id, step = 6 });

    [HttpGet("/project/{id}/{step:int}")]
    public IActionResult Step(string id, int step)
    {
        var project = _demo.GetProject(id);
        if (project is null)
        {
            return NotFound();
        }

        step = Math.Clamp(step, 0, 6);
        if (step == 6 && project.Status != ProjectStatus.Scheduled)
        {
            return RedirectToAction(nameof(Step), new { id, step = 5 });
        }

        return View("Journey", new JourneyViewModel
        {
            Project = project,
            Step = step,
            Days = _demo.InstallationDays(),
            Error = TempData["JourneyError"] as string
        });
    }

    [HttpPost("/project/{id}/{step:int}")]
    [ValidateAntiForgeryToken]
    public IActionResult Advance(string id, int step, JourneyForm form)
    {
        var project = _demo.GetProject(id);
        if (project is null)
        {
            return NotFound();
        }

        if (step == 2 && !form.ScopeAccepted)
        {
            TempData["JourneyError"] = "scope";
            return RedirectToAction(nameof(Step), new { id, step });
        }

        _demo.Update(id, current =>
        {
            if (!string.IsNullOrWhiteSpace(form.PropertyType))
            {
                current.PropertyType = form.PropertyType;
            }

            if (!string.IsNullOrWhiteSpace(form.InspectPath))
            {
                current.InspectPath = form.InspectPath;
            }

            if (form.PaymentPath is { } path)
            {
                current.PaymentPath = path;
            }
        });

        if (step == 2)
        {
            _demo.ApproveScope(id);
        }

        if (step == 3)
        {
            _demo.MarkPaymentPending(id);
        }

        if (step == 4)
        {
            _demo.AuthorizeSandboxPayment(id);
        }

        if (step == 5)
        {
            if (form.Date is null)
            {
                TempData["JourneyError"] = "date";
                return RedirectToAction(nameof(Step), new { id, step });
            }

            var booked = _demo.TrySchedule(id, form.Date.Value);
            if (!booked.Ok)
            {
                TempData["JourneyError"] = booked.Error;
                return RedirectToAction(nameof(Step), new { id, step });
            }

            return RedirectToAction(nameof(Step), new { id, step = 6 });
        }

        return RedirectToAction(nameof(Step), new { id, step = Math.Min(6, step + 1) });
    }
}
