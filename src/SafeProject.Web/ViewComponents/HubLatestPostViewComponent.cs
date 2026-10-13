using Microsoft.AspNetCore.Mvc;
using SafeProject.Web.Infrastructure;

namespace SafeProject.Web.ViewComponents;

public sealed class HubLatestPostViewComponent : ViewComponent
{
    private readonly HubLatestPostClient _client;

    public HubLatestPostViewComponent(HubLatestPostClient client) => _client = client;

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var post = await _client.GetAsync();
        return post is null
            ? Content("")
            : View(post);
    }
}
