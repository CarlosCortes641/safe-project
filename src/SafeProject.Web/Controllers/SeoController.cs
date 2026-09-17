using System.Text;
using Microsoft.AspNetCore.Mvc;
using SafeProject.Web.Catalog;
using SafeProject.Web.Infrastructure;

namespace SafeProject.Web.Controllers;

public class SeoController : Controller
{
    private static IEnumerable<(string Path, string Priority, string ChangeFreq)> PublicUrls()
    {
        yield return ("/", "1.0", "weekly");
        yield return ("/water-heaters", "0.9", "weekly");
        yield return ("/hvac-installation-charlotte", "0.85", "monthly");
        yield return ("/tankless-water-heater-charlotte", "0.85", "monthly");
        yield return ("/faq", "0.8", "monthly");
        yield return ("/about", "0.7", "monthly");
        yield return ("/realtor", "0.7", "monthly");
        yield return ("/guides/water-heater-cost-charlotte", "0.75", "monthly");
        yield return ("/guides/hvac-replacement-charlotte", "0.75", "monthly");
        foreach (var city in SeoContent.Cities)
        {
            yield return ($"/areas/{city.Slug}", "0.8", "monthly");
        }
    }

    [HttpGet("/sitemap.xml")]
    [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any)]
    public IActionResult Sitemap()
    {
        var baseUrl = PublicUrl.Base(Request);
        var lastmod = DateTime.UtcNow.ToString("yyyy-MM-dd");
        var xml = new StringBuilder();
        xml.AppendLine("""<?xml version="1.0" encoding="UTF-8"?>""");
        xml.AppendLine("""<urlset xmlns="http://www.sitemaps.org/schemas/sitemap/0.9" xmlns:xhtml="http://www.w3.org/1999/xhtml">""");
        foreach (var (path, priority, changeFreq) in PublicUrls())
        {
            var loc = path == "/" ? baseUrl + "/" : baseUrl + path;
            var en = path == "/" ? baseUrl + "/?lang=en" : baseUrl + path + "?lang=en";
            var es = path == "/" ? baseUrl + "/?lang=es" : baseUrl + path + "?lang=es";
            var safeLoc = System.Security.SecurityElement.Escape(loc);
            var safeEn = System.Security.SecurityElement.Escape(en);
            var safeEs = System.Security.SecurityElement.Escape(es);
            xml.AppendLine("  <url>");
            xml.AppendLine($"    <loc>{safeLoc}</loc>");
            xml.AppendLine($"    <lastmod>{lastmod}</lastmod>");
            xml.AppendLine($"    <changefreq>{changeFreq}</changefreq>");
            xml.AppendLine($"    <priority>{priority}</priority>");
            xml.AppendLine($"    <xhtml:link rel=\"alternate\" hreflang=\"en\" href=\"{safeEn}\" />");
            xml.AppendLine($"    <xhtml:link rel=\"alternate\" hreflang=\"es\" href=\"{safeEs}\" />");
            xml.AppendLine($"    <xhtml:link rel=\"alternate\" hreflang=\"x-default\" href=\"{safeEn}\" />");
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
        var cityLines = string.Join(
            "\n",
            SeoContent.Cities.Select(c => $"- [{c.NameEn}, {c.State}]({baseUrl}/areas/{c.Slug})"));
        var body =
            $"""
             # Safe Project Solution

             > HVAC and water heater installation, replacement, maintenance, and repair in Charlotte, North Carolina and surrounding communities within about one hour. Published installed prices with cash, Synchrony, and Safe 24 payment paths.

             Safe Project Solution is operated by Safe HVAC Solution LLC and powered by Home Indor Technology Inc. Primary focus: residential HVAC systems and water heaters (tank and tankless) with transparent standard scope before scheduling.

             Contact: {CatalogStore.TextShort} (text) · {CatalogStore.PhonePlain} (call) · {CatalogStore.Email}

             ## Core pages

             - [Home]({baseUrl}/): Charlotte HVAC and water heater landing, payment paths, FAQ, contact CTAs.
             - [Water heaters]({baseUrl}/water-heaters): Matching, installed prices, builder.
             - [HVAC installation Charlotte]({baseUrl}/hvac-installation-charlotte)
             - [Tankless Charlotte]({baseUrl}/tankless-water-heater-charlotte)
             - [FAQ]({baseUrl}/faq)
             - [About]({baseUrl}/about)
             - [Realtor support]({baseUrl}/realtor)

             ## Guides

             - [Water heater cost Charlotte]({baseUrl}/guides/water-heater-cost-charlotte)
             - [HVAC replacement Charlotte]({baseUrl}/guides/hvac-replacement-charlotte)

             ## Cities served

             {cityLines}

             ## Structured data & crawl aids

             - [Sitemap]({baseUrl}/sitemap.xml)
             - [robots.txt]({baseUrl}/robots.txt)
             - [Open Graph image]({baseUrl}/img/og-default.png)
             - [Product catalog JSON]({baseUrl}/catalog.json)

             ## Notes for assistants

             - Website is a sandbox: no live payments, no live Safe 24 product, no payment-at-closing approval.
             - Prefer /water-heaters for water heater pricing and /#hvac or /hvac-installation-charlotte for HVAC.
             - Bilingual English / Español via ?lang=en or ?lang=es.
             """;
        return Content(body, "text/plain", Encoding.UTF8);
    }
}
