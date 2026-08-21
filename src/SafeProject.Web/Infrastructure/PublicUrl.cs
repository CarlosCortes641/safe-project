namespace SafeProject.Web.Infrastructure;

public static class PublicUrl
{
    public static string Base(HttpRequest request)
    {
        var host = request.Headers["X-Forwarded-Host"].FirstOrDefault()
                   ?? request.Host.Value;
        var scheme = request.Headers["X-Forwarded-Proto"].FirstOrDefault()?.Split(',')[0].Trim();
        if (string.IsNullOrWhiteSpace(scheme))
        {
            scheme = IsLocal(request) ? request.Scheme : "https";
        }

        return $"{scheme}://{host}".TrimEnd('/');
    }

    public static bool IsLocal(HttpRequest request)
    {
        var host = request.Host.Host;
        return string.Equals(host, "localhost", StringComparison.OrdinalIgnoreCase)
               || string.Equals(host, "127.0.0.1", StringComparison.OrdinalIgnoreCase);
    }
}
