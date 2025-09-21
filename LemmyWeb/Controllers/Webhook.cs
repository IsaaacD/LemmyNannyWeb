using LemmyWeb.Hubs;
using LemmyWeb.Models;
using Markdig;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace LemmyWeb.Controllers
{

    public class Webhook : Controller
    {
        public static string PROCESSED_KEY = "Processed";
        public static string COMMENTS_FROM_LEMMY = "Comments_From_Lemmy";
        public static string POSTS_FROM_LEMMY = "Posts_From_Lemmy";
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

        [Route("post")]
        [HttpPost]
        public async Task PostBodyFromLemmy()
        {
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
            _memoryCache.Set(POSTS_FROM_LEMMY, memoryProcessed);

        }

        [Route("comment")]
        [HttpPost]
        public async Task CommentBodyFromLemmy(string data)
        {
            var converted = JsonSerializer.Serialize(data);
            var memoryProcessed = new List<string>();
            // Look for cache key.
            if (!_memoryCache.TryGetValue(COMMENTS_FROM_LEMMY, out memoryProcessed))
            {
                memoryProcessed = new List<string>();
            }


            //foreach (var name in keyValuePairs)
            //{
            //    memoryProcessed!.Add($"{name.Key}={JsonSerializer.Serialize(name.Value)}");
            //}


            //string? postData = null;
            //using (var reader = new StreamReader(HttpContext.Request.Body))
            //{
            //    postData = await reader.ReadToEndAsync();

            //}


            memoryProcessed!.Add(converted);
            _memoryCache.Set(COMMENTS_FROM_LEMMY, memoryProcessed);
        }


        public class RawData
        {
            public DateTime Timestamp { get; }
            public string? Operation { get; }
            public string Schema { get; }
            public string Table { get; }

            // This assumes you have a generic type TData defined elsewhere.
            // Replace 'object' with the actual type based on your 'TData'.
            public JsonObject Data { get; }

            public JsonObject? Previous { get; } // Using nullable object? to match PHP null

            public RawData(DateTime timestamp, string operation, string schema, string table, JsonObject data, JsonObject? previous)
            {
                Timestamp = timestamp;
                Operation = operation;
                Schema = schema;
                Table = table;
                Data = data;
                Previous = previous;
            }
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
            if (!_memoryCache.TryGetValue(PROCESSED_KEY, out memoryProcessed))
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
            _memoryCache.Set(PROCESSED_KEY, memoryProcessed);


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
