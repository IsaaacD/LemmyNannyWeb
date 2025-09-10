using LemmyWeb.Controllers;
using Microsoft.AspNetCore.SignalR;

namespace LemmyWeb.Hubs
{
    public class ProcessedHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
