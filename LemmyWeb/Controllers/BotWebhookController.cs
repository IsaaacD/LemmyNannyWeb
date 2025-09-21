using LemmyWeb.Hubs;
using LemmyWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using System.Text.Json;
using System.Text.Json.Serialization;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
        public async Task PostBodyFromLemmy(object? data)
        {

            var converted = JsonSerializer.Serialize(data);

            string? postData = null;

            using (var reader = new StreamReader(HttpContext.Request.Body))
            {
                postData = await reader.ReadToEndAsync();
            }

            var memoryProcessed = new List<string>();

            // Look for cache key.

            if (!_memoryCache.TryGetValue(POSTS_FROM_LEMMY, out memoryProcessed))
            {

                memoryProcessed = new List<string>();

            }

            memoryProcessed!.Add(postData);
            memoryProcessed!.Add(converted);

            _memoryCache.Set(POSTS_FROM_LEMMY, memoryProcessed);
            await _botHub.Clients.All.SendAsync(POSTS_FROM_LEMMY, postData);
        }

        [Route("comment")]
        [HttpPost]
        public async Task CommentBodyFromLemmy(object? comment)
        {
            var converted = JsonSerializer.Serialize(comment);

            var memoryProcessed = new List<string>();

            // Look for cache key.
            if (!_memoryCache.TryGetValue(COMMENTS_FROM_LEMMY, out memoryProcessed))
            {
                memoryProcessed = new List<string>();
            }

            string? postData = null;
            using (var reader = new StreamReader(HttpContext.Request.Body))
            {
                postData = await reader.ReadToEndAsync();
            }
            memoryProcessed!.Add(postData);
            memoryProcessed!.Add(converted);

            _memoryCache.Set(COMMENTS_FROM_LEMMY, memoryProcessed);
            await _botHub.Clients.All.SendAsync(COMMENTS_FROM_LEMMY, postData);
        }
    }
}
