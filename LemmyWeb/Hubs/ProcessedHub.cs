using LemmyWeb.Controllers;
using LemmyWeb.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;

namespace LemmyWeb.Hubs
{
    public class ProcessedHub : Hub
    {
        public static string USERS_KEY = "users";

        private readonly IMemoryCache _memoryCache;
        public ProcessedHub(IMemoryCache cache)
        {
            _memoryCache = cache;            
        }

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public override async Task OnConnectedAsync()
        {
            var numberUsers = 0;
            if (!_memoryCache.TryGetValue(USERS_KEY, out numberUsers))
            {
                numberUsers = 0;
            }
            await Clients.All.SendAsync("ViewerCount", ++numberUsers);

            _memoryCache.Set(USERS_KEY, numberUsers);

            var processeds = new List<Processed>();
            if (!_memoryCache.TryGetValue(HostWebhookController.PROCESSED_KEY, out processeds))
            {
                processeds = new List<Processed>();
            }
            await Clients.Caller.SendAsync("ReceivedInitial", processeds);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var numberUsers = 0;
            if (!_memoryCache.TryGetValue(USERS_KEY, out numberUsers))
            {
                numberUsers = 0;
            }
            await Clients.All.SendAsync("ViewerCount", --numberUsers);

            _memoryCache.Set(USERS_KEY, numberUsers);
        }
    }
}
