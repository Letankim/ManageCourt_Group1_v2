using Microsoft.AspNetCore.SignalR;

namespace WEB_ManageCourt.Hubs
{
    public class ChatHub :  Hub
    {
        public async Task JoinGroup(string userId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, userId);
        }

        public async Task SendMessageToAdmin(string userId, string message)
        {
            await Clients.Group("Admin").SendAsync("ReceiveMessage", userId, message, false);
        }
        public async Task SendMessageToUser(string userId, string message)
        {
            await Clients.Group(userId).SendAsync("ReceiveMessage", userId, message, true);
        }
    }
}
