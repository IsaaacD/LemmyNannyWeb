using LemmyWeb.Controllers;
using LemmyWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json.Nodes;
using static LemmyWeb.Controllers.Webhook;

namespace LemmyWeb.Pages
{
    public class WebhookViewerModel : PageModel
    {
        private readonly IMemoryCache _memoryCache;

        public List<JsonObject> Posts { get; set; } = new List<JsonObject>();
        public List<JsonObject> Comments { get; set; } = new List<JsonObject>();
        public WebhookViewerModel(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }
        public void OnGet()
        {
            var comments = new List<JsonObject>();
            if (!_memoryCache.TryGetValue(Webhook.COMMENTS_FROM_LEMMY, out comments))
            {
                comments = new List<JsonObject>();
            }
            Comments = comments!;

            var posts = new List<JsonObject>();
            if (!_memoryCache.TryGetValue(Webhook.POSTS_FROM_LEMMY, out posts))
            {
                posts = new List<JsonObject>();
            }
            Posts = posts!;
        }
    }
}
