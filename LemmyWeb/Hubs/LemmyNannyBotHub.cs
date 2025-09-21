using LemmyWeb.Controllers;
using LemmyWeb.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;

namespace LemmyWeb.Hubs
{
    
    public class LemmyNannyBotHub : Hub
    {
        private readonly IMemoryCache _memoryCache;
        public LemmyNannyBotHub(IMemoryCache cache)
        {
            _memoryCache = cache;
        }

        [HubMethodName("")]
        public async Task PostMessage(string message)
        {
            await Clients.Caller.SendAsync("", message);
        }

        public override async Task OnConnectedAsync()
        {
            var posts = new List<Post>();
            if (!_memoryCache.TryGetValue(BotWebhookController.POSTS_FROM_LEMMY, out posts))
            {
                posts = new List<Post>();
            }
            await Clients.Caller.SendAsync("Initial_Posts", posts);

            var comments = new List<Comment>();
            if (!_memoryCache.TryGetValue(BotWebhookController.COMMENTS_FROM_LEMMY, out comments))
            {
                comments = new List<Comment>();
            }
            await Clients.Caller.SendAsync("Initial_Comments", comments);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }
    }
}
