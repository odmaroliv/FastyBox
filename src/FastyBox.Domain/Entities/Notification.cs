using FastyBox.Domain.Common;
using FastyBox.Domain.Enums;

namespace FastyBox.Domain.Entities
{
    public class Notification : BaseTenantEntity
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
        public DateTime? ReadAt { get; set; }
        public bool IsRead => ReadAt.HasValue;
        public NotificationType Type { get; set; }
        public string TargetUrl { get; set; }
        public Guid? ShipmentId { get; set; }
        public virtual Shipment Shipment { get; set; }
    }
}
