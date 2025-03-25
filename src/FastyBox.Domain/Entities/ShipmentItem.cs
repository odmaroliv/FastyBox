using FastyBox.Domain.Common;

namespace FastyBox.Domain.Entities
{
    public class ShipmentItem : BaseTenantEntity
    {
        public Guid ShipmentId { get; set; }
        public virtual Shipment Shipment { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal Weight { get; set; }
    }
}
