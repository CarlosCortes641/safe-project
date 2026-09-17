using System.Text;
using Microsoft.AspNetCore.Mvc;
using SafeProject.Web.Catalog;
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
        xml.AppendLine("""<urlset xmlns="http://www.sitemaps.org/schemas/sitemap/0.9" xmlns:xhtml="http://www.w3.org/1999/xhtml">""");
        foreach (var (path, priority, changeFreq) in PublicUrls)
        {
            var loc = path == "/" ? baseUrl + "/" : baseUrl + path;
            var safeLoc = System.Security.SecurityElement.Escape(loc);
            xml.AppendLine("  <url>");
            xml.AppendLine($"    <loc>{safeLoc}</loc>");
            xml.AppendLine($"    <lastmod>{lastmod}</lastmod>");
            xml.AppendLine($"    <changefreq>{changeFreq}</changefreq>");
            xml.AppendLine($"    <priority>{priority}</priority>");
            xml.AppendLine($"    <xhtml:link rel=\"alternate\" hreflang=\"en\" href=\"{safeLoc}\" />");
            xml.AppendLine($"    <xhtml:link rel=\"alternate\" hreflang=\"es\" href=\"{safeLoc}\" />");
            xml.AppendLine($"    <xhtml:link rel=\"alternate\" hreflang=\"x-default\" href=\"{safeLoc}\" />");
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
             Disallow: /Home/Error
             Disallow: /realtor/*/received

             Sitemap: {baseUrl}/sitemap.xml
             """;
        return Content(body, "text/plain", Encoding.UTF8);
    }

    [HttpGet("/llms.txt")]
    [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any)]
    public IActionResult Llms()
    {
        var baseUrl = PublicUrl.Base(Request);
        var body =
            $"""
             # Safe Project Solution

             > HVAC and water heater installation, replacement, maintenance, and repair in Charlotte, North Carolina and surrounding communities within about one hour. Published installed prices with cash, Synchrony, and Safe 24 payment paths.

             Safe Project Solution is operated by Safe HVAC Solution LLC and powered by Home Indor Technology Inc. Primary focus: residential HVAC systems and water heaters (tank and tankless) with transparent standard scope before scheduling.

             Contact: {CatalogStore.TextShort} (text) · {CatalogStore.PhonePlain} (call) · {CatalogStore.Email}

             ## Core pages

             - [Home — HVAC & water heaters overview]({baseUrl}/): Local SEO landing for Charlotte HVAC and water heater installation, payment paths, FAQ, and contact CTAs.
             - [Water heater installation]({baseUrl}/water-heaters): Electric, gas, and tankless matching, published installed prices, online price builder, and standard scope.
             - [Realtor / closing support]({baseUrl}/realtor): Specialist visit request for Realtors, sellers, buyers, investors, and lenders.

             ## Structured data & crawl aids

             - [Sitemap]({baseUrl}/sitemap.xml)
             - [robots.txt]({baseUrl}/robots.txt)
             - [Product catalog JSON]({baseUrl}/catalog.json): Machine-readable offer list (sandbox; not for direct checkout).

             ## Service area

             Charlotte, NC metro and communities approximately one hour away, including nearby cities in North Carolina and Rock Hill, SC area coverage intent.

             ## Notes for assistants

             - Website is a sandbox: no live payments, no live Safe 24 product, no payment-at-closing approval.
             - Prefer linking customers to /water-heaters for water heater pricing and /#hvac for HVAC quick quote.
             - Bilingual English / Español via on-site language toggle.
             """;
        return Content(body, "text/plain", Encoding.UTF8);
    }
}
