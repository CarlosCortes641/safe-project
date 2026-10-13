using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.Caching.Memory;

namespace SafeProject.Web.Infrastructure;

public sealed class HubLatestPost
{
    public string Title { get; set; } = "";
    public string Body { get; set; } = "";
    public string? ImageUrl { get; set; }
    public DateTimeOffset PublishedUtc { get; set; }
}

public sealed class HubLatestPostClient
{
    public const string HttpClientName = "IndorNews";

    private static readonly TimeSpan CacheFor = TimeSpan.FromMinutes(1);

    private readonly IHttpClientFactory _http;
    private readonly IMemoryCache _cache;
    private readonly string _site;

    public HubLatestPostClient(IHttpClientFactory http, IMemoryCache cache, IConfiguration config)
    {
        _http = http;
        _cache = cache;
        _site = config["Updates:SiteKey"] ?? "safecomfort";
    }

    public async Task<HubLatestPost?> GetAsync()
    {
        var cacheKey = "hub-latest-post:" + _site;
        if (_cache.TryGetValue(cacheKey, out HubLatestPost? cached))
        {
            return cached;
        }

        HubLatestPost? post = null;
        try
        {
            var client = _http.CreateClient(HttpClientName);
            using var response = await client.GetAsync("api/site-posts/" + Uri.EscapeDataString(_site));
            if (response.StatusCode == HttpStatusCode.OK)
            {
                post = await response.Content.ReadFromJsonAsync<HubLatestPost>();
                if (string.IsNullOrWhiteSpace(post?.Title))
                {
                    post = null;
                }
            }
        }
        catch
        {
            post = null;
        }

        _cache.Set(cacheKey, post, CacheFor);
        return post;
    }
}
