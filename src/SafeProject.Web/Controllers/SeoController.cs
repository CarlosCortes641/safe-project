using System.Text;
using Microsoft.AspNetCore.Mvc;
using SafeProject.Web.Infrastructure;

namespace SafeProject.Web.Controllers;

public class SeoController : Controller
{
    private static readonly (string Path, string Priority, string ChangeFreq)[] PublicUrls =
    [
        ("/", "1.0", "weekly"),
        ("/water-heaters", "0.9", "weekly"),
        ("/realtor", "0.7", "monthly")
    ];

    [HttpGet("/sitemap.xml")]
    [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any)]
    public IActionResult Sitemap()
    {
        var baseUrl = PublicUrl.Base(Request);
        var lastmod = DateTime.UtcNow.ToString("yyyy-MM-dd");
        var xml = new StringBuilder();
        xml.AppendLine("""<?xml version="1.0" encoding="UTF-8"?>""");
        xml.AppendLine("""<urlset xmlns="http://www.sitemaps.org/schemas/sitemap/0.9">""");
        foreach (var (path, priority, changeFreq) in PublicUrls)
        {
            var loc = path == "/" ? baseUrl + "/" : baseUrl + path;
            xml.AppendLine("  <url>");
            xml.AppendLine($"    <loc>{System.Security.SecurityElement.Escape(loc)}</loc>");
            xml.AppendLine($"    <lastmod>{lastmod}</lastmod>");
            xml.AppendLine($"    <changefreq>{changeFreq}</changefreq>");
            xml.AppendLine($"    <priority>{priority}</priority>");
            xml.AppendLine("  </url>");
        }

        xml.AppendLine("</urlset>");
        return Content(xml.ToString(), "application/xml", Encoding.UTF8);
    }

    [HttpGet("/robots.txt")]
    [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any)]
    public IActionResult Robots()
    {
        var baseUrl = PublicUrl.Base(Request);
        var body =
            $"""
             User-agent: *
             Allow: /
             Disallow: /ops
             Disallow: /project
             Disallow: /catalog.json
             Disallow: /lang/
             Disallow: /health

             Sitemap: {baseUrl}/sitemap.xml
             """;
        return Content(body, "text/plain", Encoding.UTF8);
    }
}
