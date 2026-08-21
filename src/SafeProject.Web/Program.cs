using System.Globalization;
using Microsoft.AspNetCore.HttpOverrides;
using SafeProject.Web.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<SafeProject.Web.Catalog.CatalogStore>();
builder.Services.AddSingleton<SafeProject.Web.Demo.DemoStore>();
builder.Services.AddScoped<SafeProject.Web.Infrastructure.SiteLanguage>();
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

var app = builder.Build();

app.UseForwardedHeaders();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
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

// Lightweight probe for Render Free — avoids compiling/rendering the full home page on boot.
app.MapGet("/health", () => Results.Text("ok"));

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
