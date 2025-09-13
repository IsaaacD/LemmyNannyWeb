using LemmyWeb.Hubs;
using LemmyWeb.Models;
using Markdig;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace LemmyWeb.Controllers
{

    public class Webhook : Controller
    {
        public static List<Processed> Processeds = [];
        public static LemmNannyStats LemmNannyCurrentStats = new();

        private readonly IHubContext<ProcessedHub> _hubContext;
        private readonly string _secretKey;
        public Webhook(IHubContext<ProcessedHub> processedHub, IConfiguration config)
        {
            _hubContext = processedHub;
            _secretKey = config["SecretKey"] ?? throw new Exception("SecretKey not set");
        }

        [Route("webhook")]
        [HttpPost]
        public async Task<string> PostAsync([FromBody] Processed value)
        {
            if (Request.Headers.ContainsKey("ClientSecret"))
            {
                if (Request.Headers["ClientSecret"].ToString() == _secretKey)
                {
                    Processeds.Add(value);
                    if (Processeds.Count > 50)
                        Processeds.RemoveAt(0);
                    value.Content = Markdown.ToHtml(value.Content ?? "");
                    value.Reason = Markdown.ToHtml(value.Reason ?? "");
                    await _hubContext.Clients.All.SendAsync("ReceiveProcessed", value);
                    LemmNannyCurrentStats.LastSeen = DateTime.UtcNow;
                    switch (value.ProcessedType)
                    {
                        case ProcessedType.Comment:
                            if (value.IsReported)
                            {
                                LemmNannyCurrentStats.CommentsFlagged += 1;
                            }
                            LemmNannyCurrentStats.CommentsProcessed += 1;
                            break;
                        case ProcessedType.Post:
                            if (value.IsReported)
                            {
                                LemmNannyCurrentStats.PostsFlagged += 1;
                            }
                            LemmNannyCurrentStats.PostsProcessed += 1;
                            break;
                    }
                }
            }

            return await Task.FromResult(string.Empty);
        }

        [Route("startup")]
        [HttpPost]
        public void PostStartup([FromBody] LemmNannyStats stats)
        {
            if (Request.Headers.ContainsKey("ClientSecret"))
            {
                if (Request.Headers["ClientSecret"].ToString() == _secretKey)
                {
                    LemmNannyCurrentStats = stats;
                    LemmNannyCurrentStats.IsSet = true;
                    LemmNannyCurrentStats.LastSeen = DateTime.UtcNow;
                }
            }
        }
    }
}
