using FastyBox.Web.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace FastyBox.Web.Services
{
    public interface INotificationHubService
    {
        Task NotifyUserAsync(string userId, string method, object payload);
        Task NotifyAllAsync(string method, object payload);
        Task NotifyGroupAsync(string group, string method, object payload);
    }

    public class NotificationHubService : INotificationHubService
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationHubService(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task NotifyUserAsync(string userId, string method, object payload)
        {
            await _hubContext.Clients.User(userId).SendAsync(method, payload);
        }

        public async Task NotifyAllAsync(string method, object payload)
        {
            await _hubContext.Clients.All.SendAsync(method, payload);
        }

        public async Task NotifyGroupAsync(string group, string method, object payload)
        {
            await _hubContext.Clients.Group(group).SendAsync(method, payload);
        }
    }

}
