namespace SafeProject.Web.Infrastructure;

public sealed class SiteLanguage
{
    public const string CookieName = "safe.lang";

    public SiteLanguage(IHttpContextAccessor accessor)
    {
        var request = accessor.HttpContext?.Request;
        var langQ = request?.Query["lang"].FirstOrDefault();
        if (string.Equals(langQ, "es", StringComparison.OrdinalIgnoreCase))
        {
            IsSpanish = true;
        }
        else if (string.Equals(langQ, "en", StringComparison.OrdinalIgnoreCase))
        {
            IsSpanish = false;
        }
        else
        {
            var value = request?.Cookies[CookieName];
            IsSpanish = string.Equals(value, "es", StringComparison.OrdinalIgnoreCase);
        }
    }

    public bool IsSpanish { get; }
    public string HtmlLang => IsSpanish ? "es" : "en";
    public string OtherCode => IsSpanish ? "en" : "es";
    public string ToggleLabel => IsSpanish ? "EN" : "ES";

    public string T(string english, string spanish) => IsSpanish ? spanish : english;
}
