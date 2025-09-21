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

        public List<Post> Posts { get; set; } = new List<Post>();
        public List<Comment> Comments { get; set; } = new List<Comment>();
        public WebhookViewerModel(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }
        public void OnGet()
        {
            var comments = new List<Comment>();
            if (!_memoryCache.TryGetValue(BotWebhookController.COMMENTS_FROM_LEMMY, out comments))
            {
                comments = new List<Comment>();
            }
            Comments = comments!;

            var posts = new List<Post>();
            if (!_memoryCache.TryGetValue(BotWebhookController.POSTS_FROM_LEMMY, out posts))
            {
                posts = new List<Post>();
            }
            Posts = posts!;
        }
    }
}
