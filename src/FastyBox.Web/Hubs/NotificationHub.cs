using FastyBox.Application.Common.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace FastyBox.Web.Hubs
{
    public class NotificationHub : Hub
    {
        private readonly ICurrentUserService _currentUserService;

        public NotificationHub(ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
        }

        public override async Task OnConnectedAsync()
        {
            if (_currentUserService.IsAuthenticated)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, _currentUserService.UserId);

                if (_currentUserService.TenantId.HasValue)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, $"tenant_{_currentUserService.TenantId}");
                }

                foreach (var role in _currentUserService.Roles)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, $"role_{role}");
                }
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            if (_currentUserService.IsAuthenticated)
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, _currentUserService.UserId);

                if (_currentUserService.TenantId.HasValue)
                {
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"tenant_{_currentUserService.TenantId}");
                }

                foreach (var role in _currentUserService.Roles)
                {
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"role_{role}");
                }
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}

