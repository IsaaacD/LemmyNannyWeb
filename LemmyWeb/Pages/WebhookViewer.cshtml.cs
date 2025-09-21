using LemmyWeb.Controllers;
using LemmyWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json.Nodes;
using static LemmyWeb.Controllers.HostWebhookController;

namespace LemmyWeb.Pages
{
    public class WebhookViewerModel : PageModel
    {
        private readonly IMemoryCache _memoryCache;

        public List<string> Posts { get; set; } = new List<string>();
        public List<string> Comments { get; set; } = new List<string>();
        public WebhookViewerModel(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }
        public void OnGet()
        {
            var comments = new List<string>();
            if (!_memoryCache.TryGetValue(BotWebhookController.COMMENTS_FROM_LEMMY, out comments))
            {
                comments = new List<string>();
            }
            Comments = comments!;

            var posts = new List<string>();
            if (!_memoryCache.TryGetValue(BotWebhookController.POSTS_FROM_LEMMY, out posts))
            {
                posts = new List<string>();
            }
            Posts = posts!;
        }
    }
}
