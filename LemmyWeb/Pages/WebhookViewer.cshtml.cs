using LemmyWeb.Controllers;
using LemmyWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;
using static LemmyWeb.Controllers.Webhook;

namespace LemmyWeb.Pages
{
    public class WebhookViewerModel : PageModel
    {
        private readonly IMemoryCache _memoryCache;

        public List<RawData> Posts { get; set; } = new List<RawData>();
        public List<RawData> Comments { get; set; } = new List<RawData>();
        public WebhookViewerModel(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }
        public void OnGet()
        {
            var comments = new List<RawData>();
            if (!_memoryCache.TryGetValue(Webhook.COMMENTS_FROM_LEMMY, out comments))
            {
                comments = new List<RawData>();
            }
            Comments = comments!;

            var posts = new List<RawData>();
            if (!_memoryCache.TryGetValue(Webhook.POSTS_FROM_LEMMY, out posts))
            {
                posts = new List<RawData>();
            }
            Posts = posts!;
        }
    }
}
