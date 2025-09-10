using LemmyWeb.Hubs;
using Markdig;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Net.Http.Headers;
using System.Text.Json;

namespace LemmyWeb.Controllers
{
    [Route("webhook")]
    public class Webhook : Controller
    {
        public static List<Processed> Processeds = [];
        private readonly IHubContext<ProcessedHub> _hubContext;
        private readonly string _secretKey;
        public Webhook(IHubContext<ProcessedHub> processedHub, IConfiguration config)
        {
            _hubContext = processedHub;
            _secretKey = config["SecretKey"] ?? throw new Exception("SecretKey not set");
        }

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
                }
            }

            return await Task.FromResult(string.Empty);
        }
    }

    public class Processed
    {
        public int PostId { get; set; }
        public int Id { get; set; }
        public string? Url { get; set; }
        public string? Reason { get; set; }
        public string? Content { get; set; }
        public string? Title { get; set; }
        public bool IsReported { get; set; }
        public List<string> History { get; set; } = [];
        public ProcessedType ProcessedType { get; set; }
        public string? Username { get; set; }
        public string? AvatarUrl { get; set; }
        public DateTime ProcessedOn { get; set; }
        public string? CreatedDate { get; set; }
        public string? PostUrl { get; set; }
        public string? ExtraInfo { get; set; }
    }
    public enum ProcessedType
    {
        NotSet,
        Comment,
        Post
    }
}
