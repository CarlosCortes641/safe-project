using Microsoft.AspNetCore.Mvc;
using SafeProject.Web.Catalog;

namespace SafeProject.Web.Controllers;

public class CatalogController : Controller
{
    private readonly CatalogStore _catalog;

    public CatalogController(CatalogStore catalog)
    {
        _catalog = catalog;
    }

    [HttpGet("/catalog.json")]
    public IActionResult Json() => new JsonResult(_catalog.ToPublicJson());
}
