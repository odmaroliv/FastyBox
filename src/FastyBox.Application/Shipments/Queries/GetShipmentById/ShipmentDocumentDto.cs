using AutoMapper;
using FastyBox.Application.Common.Mappings;
using FastyBox.Domain.Entities;
using FastyBox.Domain.Enums;

namespace FastyBox.Application.Shipments.Queries.GetShipmentById
{
    public class ShipmentDocumentDto : IMapFrom<ShipmentDocument>
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public string PublicUrl { get; set; }
        public long FileSize { get; set; }
        public DocumentType Type { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedAt { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<ShipmentDocument, ShipmentDocumentDto>();
        }
    }
}
