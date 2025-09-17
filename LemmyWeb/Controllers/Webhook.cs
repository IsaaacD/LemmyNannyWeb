using LemmyWeb.Hubs;
using LemmyWeb.Models;
using Markdig;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;

namespace LemmyWeb.Controllers
{

    public class Webhook : Controller
    {
        public static string PROCESSED_KEy = "Processed";
        public static string STATS_KEY = "Stats";

        private readonly IHubContext<ProcessedHub> _hubContext;
        private readonly string _secretKey;
        private readonly IMemoryCache _memoryCache;
        public Webhook(IHubContext<ProcessedHub> processedHub, IConfiguration config, IMemoryCache memoryCache)
        {
            _hubContext = processedHub;
            _secretKey = config["SecretKey"] ?? throw new Exception("SecretKey not set");
            _memoryCache = memoryCache;
        }

        [Route("webhook")]
        [HttpPost]
        public async Task<string> PostAsync([FromBody] Processed value)
        {
            if (Request.Headers.ContainsKey("ClientSecret"))
            {
                if (Request.Headers["ClientSecret"].ToString() != _secretKey)
                {
                    return await Task.FromResult(string.Empty);
                }
            }

            var memoryProcessed = new List<Processed>();
            // Look for cache key.
            if (!_memoryCache.TryGetValue(PROCESSED_KEy, out memoryProcessed))
            {
                memoryProcessed = new List<Processed>();
            }

            memoryProcessed!.Add(value);
            if (memoryProcessed.Count > 50)
                memoryProcessed.RemoveAt(0);
            value.Content = Markdown.ToHtml(value.Content ?? "");
            value.Reason = Markdown.ToHtml(value.Reason ?? "");
            await _hubContext.Clients.All.SendAsync("ReceiveProcessed", value);

            var stats = new LemmNannyStats();
            if (!_memoryCache.TryGetValue(STATS_KEY, out stats))
            {
                stats = new LemmNannyStats();
            }

            stats!.LastSeen = DateTime.UtcNow;
            switch (value.ProcessedType)
            {
                case ProcessedType.Comment:
                    if (value.IsReported)
                    {
                        stats.CommentsFlagged += 1;
                    }
                    stats.CommentsProcessed += 1;
                    break;
                case ProcessedType.Post:
                    if (value.IsReported)
                    {
                        stats.PostsFlagged += 1;
                    }
                    stats.PostsProcessed += 1;
                    break;
            }

            _memoryCache.Set(STATS_KEY, stats);
            _memoryCache.Set(PROCESSED_KEy, memoryProcessed);


            return await Task.FromResult(string.Empty);
        }

        [Route("startup")]
        [HttpPost]
        public async Task<string> PostStartup([FromBody] LemmNannyStats stats)
        {
            if (Request.Headers.ContainsKey("ClientSecret"))
            {
                if (Request.Headers["ClientSecret"].ToString() != _secretKey)
                {
                    return await Task.FromResult(string.Empty);
                }
            }

            stats!.IsSet = true;
            stats.LastSeen = DateTime.UtcNow;

            _memoryCache.Set(STATS_KEY, stats);

            return await Task.FromResult(string.Empty);

        }
    }
}
