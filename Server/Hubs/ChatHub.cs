using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace Server.Hubs
{
    public class ChatHub : Hub
    {

        /*public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            return base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);

        }*/
        public Task JoinRoom(string room)
        {
            return Groups.AddToGroupAsync(Context.ConnectionId, room);
        }

        public async Task SendMessageToGroup(string group, string user, string message)
        {
            await Clients.Group(group).SendAsync("Message", user, message);

        }
    }
}
