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
    public const string PhoneDisplay = "(980) 447-0919";
    public const string PhoneTel = "+19804470919";
    public const string Email = "safehvacsolution@gmail.com";
    public const string SynchronyUrl = "https://www.synchrony.com/mmc/S6239195200?sitecode=acewel402";

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

    public object ToPublicJson() => new
    {
        schema_version = "1.0",
        source = "hardcoded-catalog",
        business = new
        {
            name = "Safe Project Solution by Safe HVAC Solution LLC",
            powered_by = "Home Indor Technology Inc.",
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
