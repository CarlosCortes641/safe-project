using System.Globalization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using SafeProject.Web.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
});
builder.Services.AddSingleton<SafeProject.Web.Catalog.CatalogStore>();
builder.Services.AddSingleton<SafeProject.Web.Demo.DemoStore>();
builder.Services.AddScoped<SafeProject.Web.Infrastructure.SiteLanguage>();
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor
        | ForwardedHeaders.XForwardedProto
        | ForwardedHeaders.XForwardedHost;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
    options.RequireHeaderSymmetry = false;
    options.ForwardLimit = 2;
});
builder.Services.AddAntiforgery(options =>
{
    options.Cookie.Name = ".Safe.AntiForgery";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
});
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.MinimumSameSitePolicy = SameSiteMode.Lax;
    options.HttpOnly = Microsoft.AspNetCore.CookiePolicy.HttpOnlyPolicy.Always;
    options.Secure = CookieSecurePolicy.SameAsRequest;
});

var app = builder.Build();

app.UseForwardedHeaders();
app.UseMiddleware<SecurityHeadersMiddleware>();

if (!app.Environment.IsDevelopment())
{
    app.Use(async (context, next) =>
    {
        var proto = context.Request.Headers["X-Forwarded-Proto"].FirstOrDefault()?.Split(',')[0].Trim();
        context.Request.Scheme = string.IsNullOrWhiteSpace(proto) ? "https" : proto;
        await next();
    });
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseCookiePolicy();
app.UseStaticFiles();
app.Use(async (context, next) =>
{
    var code = string.Equals(context.Request.Cookies[SiteLanguage.CookieName], "es", StringComparison.OrdinalIgnoreCase)
        ? "es-US"
        : "en-US";
    var culture = CultureInfo.GetCultureInfo(code);
    CultureInfo.CurrentCulture = culture;
    CultureInfo.CurrentUICulture = culture;
    await next();
});
app.UseRouting();
app.UseAuthorization();

app.MapGet("/health", () => Results.Text("ok"));

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
