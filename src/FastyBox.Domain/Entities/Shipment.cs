using FastyBox.Domain.Common;
using FastyBox.Domain.Enums;

namespace FastyBox.Domain.Entities
{
    public class Shipment : BaseTenantEntity
    {
        public string TrackingNumber { get; set; }
        public string ReferenceNumber { get; set; }
        public ShipmentType Type { get; set; }
        public ShipmentStatus Status { get; set; }
        public decimal DeclaredValue { get; set; }
        public decimal Weight { get; set; }
        public int PackageCount { get; set; }
        public string Description { get; set; }
        public string CustomerNotes { get; set; }
        public string AdminNotes { get; set; }
        public bool IsPaid { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal ServiceFee { get; set; }
        public decimal TotalCost { get; set; }
        public string CourierTrackingNumber { get; set; }
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
        public Guid? SourceAddressId { get; set; }
        public virtual Address SourceAddress { get; set; }
        public Guid? DestinationAddressId { get; set; }
        public virtual Address DestinationAddress { get; set; }
        public string PaymentIntentId { get; set; }
        public virtual ICollection<ShipmentItem> Items { get; set; } = new List<ShipmentItem>();
        public virtual ICollection<ShipmentDocument> Documents { get; set; } = new List<ShipmentDocument>();
        public virtual ICollection<ShipmentStatusHistory> StatusHistory { get; set; } = new List<ShipmentStatusHistory>();
        public virtual ICollection<ShipmentNote> Notes { get; set; } = new List<ShipmentNote>();
    }

}
