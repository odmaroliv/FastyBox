using FastyBox.Application.Common.Interfaces;
using FastyBox.Domain.Entities;
using FastyBox.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace FastyBox.Infrastructure.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IApplicationDbContext _context;
        private readonly IDateTime _dateTime;

        public NotificationService(IApplicationDbContext context, IDateTime dateTime)
        {
            _context = context;
            _dateTime = dateTime;
        }

        public async Task<Notification> CreateNotificationAsync(
            string userId,
            string title,
            string message,
            NotificationType type,
            string targetUrl = null,
            Guid? shipmentId = null,
            CancellationToken cancellationToken = default)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                throw new ArgumentException("User not found", nameof(userId));
            }

            var notification = new Notification
            {
                Title = title,
                Message = message,
                UserId = userId,
                Type = type,
                TargetUrl = targetUrl,
                ShipmentId = shipmentId,
                TenantId = user.TenantId ?? Guid.Empty
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync(cancellationToken);

            return notification;
        }

        public async Task MarkAsReadAsync(Guid notificationId, CancellationToken cancellationToken = default)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification != null && notification.ReadAt == null)
            {
                notification.ReadAt = _dateTime.UtcNow;
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task MarkAllAsReadAsync(string userId, CancellationToken cancellationToken = default)
        {
            var unreadNotifications = await _context.Notifications
                .Where(n => n.UserId == userId && n.ReadAt == null)
                .ToListAsync(cancellationToken);

            foreach (var notification in unreadNotifications)
            {
                notification.ReadAt = _dateTime.UtcNow;
            }

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<IEnumerable<Notification>> GetUnreadNotificationsAsync(string userId, CancellationToken cancellationToken = default)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId && n.ReadAt == null)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<int> GetUnreadCountAsync(string userId, CancellationToken cancellationToken = default)
        {
            return await _context.Notifications
                .CountAsync(n => n.UserId == userId && n.ReadAt == null, cancellationToken);
        }
    }
}
