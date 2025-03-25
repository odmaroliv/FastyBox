using FastyBox.Domain.Entities;
using FastyBox.Domain.Enums;

namespace FastyBox.Application.Common.Interfaces
{
    public interface INotificationService
    {
        Task<Notification> CreateNotificationAsync(string userId, string title, string message, NotificationType type, string targetUrl = null, Guid? shipmentId = null, CancellationToken cancellationToken = default);
        Task MarkAsReadAsync(Guid notificationId, CancellationToken cancellationToken = default);
        Task MarkAllAsReadAsync(string userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Notification>> GetUnreadNotificationsAsync(string userId, CancellationToken cancellationToken = default);
        Task<int> GetUnreadCountAsync(string userId, CancellationToken cancellationToken = default);
    }
}
