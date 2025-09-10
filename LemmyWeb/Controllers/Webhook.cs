using LemmyWeb.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json;

namespace LemmyWeb.Controllers
{
    [Route("webhook")]
    public class Webhook : Controller
    {
        private static List<Processed> Processeds = [];
        private readonly IHubContext<ProcessedHub> _hubContext;
        public Webhook(IHubContext<ProcessedHub> processedHub)
        {
            _hubContext = processedHub;
        }

        [HttpPost]
        public async Task<string> PostAsync([FromBody] Processed value)
        {
            //ar val = JsonSerializer.Deserialize<Processed>(value);
            Processeds.Add(value);
            await _hubContext.Clients.All.SendAsync("ReceiveProcessed", value);
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
    }
    public enum ProcessedType
    {
        NotSet,
        Comment,
        Post
    }
}
