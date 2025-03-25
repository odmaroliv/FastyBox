using FastyBox.Domain.Common;

namespace FastyBox.Domain.Entities
{
    public class ShipmentNote : BaseTenantEntity
    {
        public Guid ShipmentId { get; set; }
        public virtual Shipment Shipment { get; set; }
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
        public string Note { get; set; }
        public bool IsPublic { get; set; }
        public bool IsSystemGenerated { get; set; }
    }
}
