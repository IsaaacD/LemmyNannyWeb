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
        public static StartUpStats StartUpStats = new();

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
            //ar val = JsonSerializer.Deserialize<Processed>(value);
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
                    StartUpStats.LastSeen = DateTime.UtcNow;
                }
            }

            return await Task.FromResult(string.Empty);
        }

        [Route("startup")]
        [HttpPost]
        public void PostStartup([FromBody] StartUpStats stats)
        {
            StartUpStats = stats;
            StartUpStats.IsSet = true;
            StartUpStats.LastSeen = DateTime.UtcNow;
        }
    }
}
