using FastyBox.Domain.Common;
using FastyBox.Domain.Enums;

namespace FastyBox.Domain.Entities
{
    public class ShipmentStatusHistory : BaseTenantEntity
    {
        public Guid ShipmentId { get; set; }
        public virtual Shipment Shipment { get; set; }
        public ShipmentStatus PreviousStatus { get; set; }
        public ShipmentStatus NewStatus { get; set; }
        public string Notes { get; set; }
    }
}
