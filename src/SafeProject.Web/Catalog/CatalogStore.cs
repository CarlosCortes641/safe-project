using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SafeProject.Web.Catalog;

public sealed record WaterHeaterProduct(
    string Id,
    string NameEn,
    string NameEs,
    int PriceUsd,
    bool VerificationRequired,
    string Icon);

public sealed record HvacProduct(
    string Id,
    string NameEn,
    string NameEs,
    int StartingPriceUsd,
    int? ReferencePriceUsd,
    bool DuctworkIncluded);

public sealed record ScheduleDay(DateOnly Date, bool IsFull);

public sealed class CatalogStore
{
    public const int InstallationCapacityPerDay = 6;
    public const string PhoneDisplay = "(704) 268-4104";
    public const string PhoneTel = "+17042684104";
    public const string PhoneSchema = "+1-704-268-4104";
    public const string PhonePlain = "704-268-4104";
    public const string TextDisplay = "+1 (704) 268-4104";
    public const string TextTel = "+17042684104";
    public const string TextShort = "704-268-4104";
    public const string Email = "safehvacsolution@gmail.com";
    public const string BrandName = "Safe Project Solution";
    public const string LegalName = "Safe HVAC Solution LLC";
    public const string PoweredBy = "Home Indor Technology Inc.";
    public const string SynchronyUrl = "https://www.synchrony.com/mmc/S6239195200?sitecode=acewel402";

    private static readonly JsonSerializerOptions JsonLdOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = false
    };

    private readonly ConcurrentDictionary<string, string> _jsonLdCache = new(StringComparer.Ordinal);

    public IReadOnlyList<WaterHeaterProduct> WaterHeaters { get; } =
    [
        new("wh-electric-40-50", "Electric 40/50 gal", "Eléctrico 40/50 gal", 1750, false, "⚡"),
        new("wh-electric-60", "Electric 60 gal", "Eléctrico 60 gal", 1850, false, "⚡"),
        new("wh-gas-40-50", "Gas 40/50 gal", "Gas 40/50 gal", 1850, false, "🔥"),
        new("wh-gas-60", "Gas 60 gal", "Gas 60 gal", 2100, false, "🔥"),
        new("wh-electric-tankless", "Electric tankless", "Tankless eléctrico", 3200, true, "∞"),
        new("wh-gas-tankless-conversion", "Gas tankless conversion", "Conversión tankless gas", 4050, true, "∞"),
        new("wh-premium-tankless", "Premium tankless", "Tankless premium", 4750, true, "∞")
    ];

    public IReadOnlyList<HvacProduct> Hvac { get; } =
    [
        new("hvac-replace-up-to-2-5", "Complete AC + heat up to 2.5 tons", "HVAC completo hasta 2.5 toneladas", 8000, null, false),
        new("hvac-replace-3-4", "Complete AC + heat 3–4 tons", "HVAC completo 3–4 toneladas", 10000, 11000, false)
    ];

    /// <summary>
    /// Schema.org JSON-LD for GEO/AI SEO — mirrors Verified Service Facts (identity, area, water heater, HVAC).
    /// </summary>
    public string ToJsonLd(string siteUrl)
    {
        var root = siteUrl.TrimEnd('/');
        return _jsonLdCache.GetOrAdd("business:" + root, _ => BuildBusinessJsonLd(root));
    }

    private string BuildBusinessJsonLd(string root)
    {
        var businessId = $"{root}/#business";
        var orgId = $"{root}/#organization";
        var waterHeaterServiceId = $"{root}/#service-water-heater";
        var hvacServiceId = $"{root}/#service-hvac";

        var graph = new object[]
        {
            new Dictionary<string, object?>
            {
                ["@type"] = new[] { "HVACBusiness", "LocalBusiness", "Organization" },
                ["@id"] = businessId,
                ["name"] = BrandName,
                ["legalName"] = LegalName,
                ["alternateName"] = $"{BrandName} by {LegalName}",
                ["description"] =
                    "HVAC and water heater installation, replacement, maintenance, and repair services in Charlotte, North Carolina and surrounding areas.",
                ["url"] = root + "/",
                ["telephone"] = PhoneSchema,
                ["email"] = Email,
                ["priceRange"] = "$$",
                ["currenciesAccepted"] = "USD",
                ["paymentAccepted"] = "Cash, Credit Card, Financing",
                ["availableLanguage"] = new[]
                {
                    new Dictionary<string, object?> { ["@type"] = "Language", ["name"] = "English" },
                    new Dictionary<string, object?> { ["@type"] = "Language", ["name"] = "Spanish" }
                },
                ["address"] = new Dictionary<string, object?>
                {
                    ["@type"] = "PostalAddress",
                    ["addressLocality"] = "Charlotte",
                    ["addressRegion"] = "NC",
                    ["addressCountry"] = "US"
                },
                ["geo"] = new Dictionary<string, object?>
                {
                    ["@type"] = "GeoCoordinates",
                    ["latitude"] = 35.2271,
                    ["longitude"] = -80.8431
                },
                ["areaServed"] = new object[]
                {
                    new Dictionary<string, object?>
                    {
                        ["@type"] = "City",
                        ["name"] = "Charlotte",
                        ["containedInPlace"] = new Dictionary<string, object?>
                        {
                            ["@type"] = "State",
                            ["name"] = "North Carolina"
                        }
                    },
                    new Dictionary<string, object?>
                    {
                        ["@type"] = "GeoCircle",
                        ["name"] = "Charlotte and communities approximately one hour away",
                        ["geoMidpoint"] = new Dictionary<string, object?>
                        {
                            ["@type"] = "GeoCoordinates",
                            ["latitude"] = 35.2271,
                            ["longitude"] = -80.8431
                        },
                        // ~50 miles / 1-hour driving radius
                        ["geoRadius"] = "80467"
                    }
                },
                ["parentOrganization"] = new Dictionary<string, object?>
                {
                    ["@id"] = orgId
                },
                ["brand"] = new Dictionary<string, object?>
                {
                    ["@type"] = "Brand",
                    ["name"] = BrandName
                },
                ["knowsAbout"] = new[]
                {
                    "HVAC installation",
                    "HVAC replacement",
                    "Air conditioning",
                    "Heating systems",
                    "Water heater installation",
                    "Water heater replacement",
                    "Tankless water heaters",
                    "Property closing mechanical support"
                },
                ["hasOfferCatalog"] = new Dictionary<string, object?>
                {
                    ["@type"] = "OfferCatalog",
                    ["name"] = "Safe HVAC and water heater services",
                    ["itemListElement"] = new object[]
                    {
                        new Dictionary<string, object?>
                        {
                            ["@type"] = "OfferCatalog",
                            ["name"] = "Water heaters",
                            ["itemListElement"] = WaterHeaters.Select(p => new Dictionary<string, object?>
                            {
                                ["@type"] = "Offer",
                                ["itemOffered"] = new Dictionary<string, object?>
                                {
                                    ["@type"] = "Service",
                                    ["name"] = p.NameEn,
                                    ["serviceType"] = "Water heater installation"
                                },
                                ["price"] = p.PriceUsd.ToString(),
                                ["priceCurrency"] = "USD"
                            }).ToArray()
                        },
                        new Dictionary<string, object?>
                        {
                            ["@type"] = "OfferCatalog",
                            ["name"] = "HVAC",
                            ["itemListElement"] = Hvac.Select(p => new Dictionary<string, object?>
                            {
                                ["@type"] = "Offer",
                                ["itemOffered"] = new Dictionary<string, object?>
                                {
                                    ["@type"] = "Service",
                                    ["name"] = p.NameEn,
                                    ["serviceType"] = "HVAC installation"
                                },
                                ["price"] = p.StartingPriceUsd.ToString(),
                                ["priceCurrency"] = "USD"
                            }).ToArray()
                        }
                    }
                },
                ["makesOffer"] = new object[]
                {
                    new Dictionary<string, object?>
                    {
                        ["@type"] = "Offer",
                        ["itemOffered"] = new Dictionary<string, object?> { ["@id"] = waterHeaterServiceId }
                    },
                    new Dictionary<string, object?>
                    {
                        ["@type"] = "Offer",
                        ["itemOffered"] = new Dictionary<string, object?> { ["@id"] = hvacServiceId }
                    }
                }
            },
            new Dictionary<string, object?>
            {
                ["@type"] = "Organization",
                ["@id"] = orgId,
                ["name"] = LegalName,
                ["legalName"] = LegalName,
                ["url"] = root + "/",
                ["telephone"] = PhoneSchema,
                ["email"] = Email,
                ["description"] =
                    $"{BrandName} by {LegalName}. Powered by {PoweredBy}.",
                ["address"] = new Dictionary<string, object?>
                {
                    ["@type"] = "PostalAddress",
                    ["addressLocality"] = "Charlotte",
                    ["addressRegion"] = "NC",
                    ["addressCountry"] = "US"
                },
                ["areaServed"] = "Charlotte, North Carolina and communities approximately one hour away"
                // sameAs: add Google Business Profile / social URLs when available
            },
            new Dictionary<string, object?>
            {
                ["@type"] = "Service",
                ["@id"] = waterHeaterServiceId,
                ["name"] = "Water heater installation and replacement",
                ["serviceType"] = "Water heater installation",
                ["provider"] = new Dictionary<string, object?> { ["@id"] = businessId },
                ["areaServed"] = new Dictionary<string, object?>
                {
                    ["@type"] = "City",
                    ["name"] = "Charlotte"
                },
                ["description"] =
                    "New equipment, expansion tank when applicable, removal, standard connections, 2-year Safe labor, 5-year manufacturer warranty. Tank and tankless options with published installed prices.",
                ["availableChannel"] = new Dictionary<string, object?>
                {
                    ["@type"] = "ServiceChannel",
                    ["serviceUrl"] = root + "/water-heaters",
                    ["servicePhone"] = new Dictionary<string, object?>
                    {
                        ["@type"] = "ContactPoint",
                        ["telephone"] = PhoneSchema,
                        ["contactType"] = "customer service",
                        ["areaServed"] = "US",
                        ["availableLanguage"] = new[] { "English", "Spanish" }
                    }
                }
            },
            new Dictionary<string, object?>
            {
                ["@type"] = "Service",
                ["@id"] = hvacServiceId,
                ["name"] = "HVAC installation and replacement",
                ["serviceType"] = "HVAC installation",
                ["provider"] = new Dictionary<string, object?> { ["@id"] = businessId },
                ["areaServed"] = new Dictionary<string, object?>
                {
                    ["@type"] = "City",
                    ["name"] = "Charlotte"
                },
                ["description"] =
                    "Cooling and heating replacement with standard labor. Ductwork is not included. 1-year Safe labor, 5-year manufacturer warranty. Sales, installation, replacement, maintenance, and repair.",
                ["availableChannel"] = new Dictionary<string, object?>
                {
                    ["@type"] = "ServiceChannel",
                    ["serviceUrl"] = root + "/#hvac",
                    ["servicePhone"] = new Dictionary<string, object?>
                    {
                        ["@type"] = "ContactPoint",
                        ["telephone"] = PhoneSchema,
                        ["contactType"] = "customer service",
                        ["areaServed"] = "US",
                        ["availableLanguage"] = new[] { "English", "Spanish" }
                    }
                }
            },
            new Dictionary<string, object?>
            {
                ["@type"] = "Organization",
                ["name"] = PoweredBy,
                ["description"] = $"Technology partner powering {BrandName}."
            }
        };

        var document = new Dictionary<string, object?>
        {
            ["@context"] = "https://schema.org",
            ["@graph"] = graph
        };

        return JsonSerializer.Serialize(document, JsonLdOptions);
    }

    /// <summary>
    /// Focused Schema.org JSON-LD for the indexable /water-heaters landing page.
    /// </summary>
    public string ToWaterHeaterPageJsonLd(string siteUrl)
    {
        var root = siteUrl.TrimEnd('/');
        return _jsonLdCache.GetOrAdd("water:" + root, _ => BuildWaterHeaterPageJsonLd(root));
    }

    private string BuildWaterHeaterPageJsonLd(string root)
    {
        var pageUrl = root + "/water-heaters";
        var businessId = $"{root}/#business";

        var document = new Dictionary<string, object?>
        {
            ["@context"] = "https://schema.org",
            ["@graph"] = new object[]
            {
                new Dictionary<string, object?>
                {
                    ["@type"] = "WebPage",
                    ["@id"] = pageUrl + "#webpage",
                    ["url"] = pageUrl,
                    ["name"] = "Water Heater Installation in Charlotte, NC",
                    ["description"] =
                        "Published installed prices for electric, gas, and tankless water heater installation and replacement in Charlotte, NC.",
                    ["isPartOf"] = new Dictionary<string, object?>
                    {
                        ["@type"] = "WebSite",
                        ["name"] = BrandName,
                        ["url"] = root + "/"
                    },
                    ["about"] = new Dictionary<string, object?> { ["@id"] = pageUrl + "#service" },
                    ["breadcrumb"] = new Dictionary<string, object?>
                    {
                        ["@type"] = "BreadcrumbList",
                        ["itemListElement"] = new object[]
                        {
                            new Dictionary<string, object?>
                            {
                                ["@type"] = "ListItem",
                                ["position"] = 1,
                                ["name"] = "Home",
                                ["item"] = root + "/"
                            },
                            new Dictionary<string, object?>
                            {
                                ["@type"] = "ListItem",
                                ["position"] = 2,
                                ["name"] = "Water heaters",
                                ["item"] = pageUrl
                            }
                        }
                    }
                },
                new Dictionary<string, object?>
                {
                    ["@type"] = "Service",
                    ["@id"] = pageUrl + "#service",
                    ["name"] = "Water heater installation and replacement",
                    ["serviceType"] = "Water heater installation",
                    ["url"] = pageUrl,
                    ["provider"] = new Dictionary<string, object?>
                    {
                        ["@type"] = "HVACBusiness",
                        ["@id"] = businessId,
                        ["name"] = BrandName,
                        ["telephone"] = PhoneSchema,
                        ["email"] = Email
                    },
                    ["areaServed"] = new Dictionary<string, object?>
                    {
                        ["@type"] = "City",
                        ["name"] = "Charlotte",
                        ["containedInPlace"] = new Dictionary<string, object?>
                        {
                            ["@type"] = "State",
                            ["name"] = "North Carolina"
                        }
                    },
                    ["description"] =
                        "New equipment, expansion tank when applicable, removal, standard connections, 2-year Safe labor, 5-year manufacturer warranty. Tank and tankless options with published installed prices.",
                    ["hasOfferCatalog"] = new Dictionary<string, object?>
                    {
                        ["@type"] = "OfferCatalog",
                        ["name"] = "Water heater installed prices",
                        ["itemListElement"] = WaterHeaters.Select(p => new Dictionary<string, object?>
                        {
                            ["@type"] = "Offer",
                            ["url"] = pageUrl + "#builder",
                            ["price"] = p.PriceUsd.ToString(),
                            ["priceCurrency"] = "USD",
                            ["itemOffered"] = new Dictionary<string, object?>
                            {
                                ["@type"] = "Service",
                                ["name"] = p.NameEn,
                                ["serviceType"] = "Water heater installation"
                            }
                        }).ToArray()
                    }
                }
            }
        };

        return JsonSerializer.Serialize(document, JsonLdOptions);
    }

    public object ToPublicJson() => new
    {
        schema_version = "1.0",
        source = "hardcoded-catalog",
        business = new
        {
            name = $"{BrandName} by {LegalName}",
            powered_by = PoweredBy,
            phone = PhoneTel,
            email = Email,
            service_area = "Charlotte, North Carolina and communities approximately one hour away"
        },
        water_heaters = WaterHeaters.Select(p => new
        {
            p.Id,
            name_en = p.NameEn,
            name_es = p.NameEs,
            standard_installed_price_usd = p.PriceUsd,
            verification_required = p.VerificationRequired
        }),
        hvac = Hvac.Select(p => new
        {
            p.Id,
            name_en = p.NameEn,
            name_es = p.NameEs,
            starting_cash_price_usd = p.StartingPriceUsd,
            regular_reference_price_usd = p.ReferencePriceUsd,
            ductwork_included = p.DuctworkIncluded
        }),
        payment_paths = new[]
        {
            new { id = "cash_sandbox", live = false, description = "Hosted checkout simulator only" },
            new { id = "synchrony_sandbox", live = false, description = "Does not open the live Synchrony merchant application" },
            new { id = "safe24_sandbox", live = false, description = "Eligibility illustration only; program not activated" }
        },
        scheduling_policy = new
        {
            default_installation_capacity_per_operating_day = InstallationCapacityPerDay,
            note = "In-memory demo capacity. Seventh booking on a full day is rejected."
        }
    };

    public IReadOnlyList<ScheduleDay> DemoInstallationDays(DateOnly today)
    {
        var days = new List<ScheduleDay>();
        var cursor = today;
        while (days.Count < 3)
        {
            cursor = cursor.AddDays(1);
            if (cursor.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
            {
                continue;
            }

            days.Add(new ScheduleDay(cursor, IsFull: days.Count == 2));
        }

        return days;
    }
}
