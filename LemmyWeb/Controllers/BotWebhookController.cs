using LemmyWeb.Hubs;
using LemmyWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;

namespace LemmyWeb.Controllers
{
    /// <summary>
    /// Comes from LemmyWebhooks to send down to LemmyNanny
    /// </summary>
    public class BotWebhookController : Controller
    {
        public static string COMMENTS_FROM_LEMMY = "Comments_From_Lemmy";
        public static string POSTS_FROM_LEMMY = "Posts_From_Lemmy";

        private readonly IHubContext<LemmyNannyBotHub> _botHub;
        private readonly IMemoryCache _memoryCache;
        public BotWebhookController(IHubContext<LemmyNannyBotHub> botHub, IMemoryCache memoryCache)
        {
            _botHub = botHub;
            _memoryCache = memoryCache;
        }
        [Route("post")]
        [HttpPost]
        public async Task PostBodyFromLemmy(Post? post)
        {
            var memoryProcessed = new List<Post>();
            // Look for cache key.
            if (!_memoryCache.TryGetValue(POSTS_FROM_LEMMY, out memoryProcessed))
            {
                memoryProcessed = new List<Post>();
            }
            if (post != null)
            {
                memoryProcessed!.Add(post!);
            }
            _memoryCache.Set(POSTS_FROM_LEMMY, memoryProcessed);
            await _botHub.Clients.All.SendAsync(POSTS_FROM_LEMMY, post);
        }

        [Route("comment")]
        [HttpPost]
        public async Task CommentBodyFromLemmy(Comment? comment)
        {
            var memoryProcessed = new List<Comment>();
            // Look for cache key.
            if (!_memoryCache.TryGetValue(COMMENTS_FROM_LEMMY, out memoryProcessed))
            {
                memoryProcessed = new List<Comment>();
            }
            if (comment != null)
            {
                memoryProcessed!.Add(comment!);
            }
            _memoryCache.Set(COMMENTS_FROM_LEMMY, memoryProcessed);

            await _botHub.Clients.All.SendAsync(COMMENTS_FROM_LEMMY, comment);
        }
    }
}
