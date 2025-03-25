using FastyBox.Domain.Common;
using FastyBox.Domain.Enums;

namespace FastyBox.Domain.Entities
{
    public class ShipmentDocument : BaseTenantEntity
    {
        public Guid ShipmentId { get; set; }
        public virtual Shipment Shipment { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public string StoragePath { get; set; }
        public string PublicUrl { get; set; }
        public long FileSize { get; set; }
        public DocumentType Type { get; set; }
        public string Notes { get; set; }
    }
}
