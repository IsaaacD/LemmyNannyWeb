using LemmyWeb.Controllers;
using Microsoft.AspNetCore.SignalR;

namespace LemmyWeb.Hubs
{
    public class ProcessedHub : Hub
    {
        private static int _numberUsers = 0;
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public override async Task OnConnectedAsync()
        {
            await Clients.All.SendAsync("ViewerCount", ++_numberUsers);
            await Clients.Caller.SendAsync("ReceivedInitial", Webhook.Processeds);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await Clients.All.SendAsync("ViewerCount", --_numberUsers);
        }
    }
}
