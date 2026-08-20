namespace SafeProject.Web.Infrastructure;

public sealed class SiteLanguage
{
    public const string CookieName = "safe.lang";

    public SiteLanguage(IHttpContextAccessor accessor)
    {
        var value = accessor.HttpContext?.Request.Cookies[CookieName];
        IsSpanish = string.Equals(value, "es", StringComparison.OrdinalIgnoreCase);
    }

    public bool IsSpanish { get; }
    public string HtmlLang => IsSpanish ? "es" : "en";
    public string OtherCode => IsSpanish ? "en" : "es";
    public string ToggleLabel => IsSpanish ? "EN" : "ES";

    public string T(string english, string spanish) => IsSpanish ? spanish : english;
}
