using AutoMapper;
using FastyBox.Application.Common.Mappings;
using FastyBox.Domain.Entities;
using FastyBox.Domain.Enums;

namespace FastyBox.Application.Shipments.Queries.GetShipmentById
{
    public class ShipmentDto : IMapFrom<Shipment>
    {
        public Guid Id { get; set; }
        public string TrackingNumber { get; set; }
        public string ReferenceNumber { get; set; }
        public ShipmentType Type { get; set; }
        public ShipmentStatus Status { get; set; }
        public decimal DeclaredValue { get; set; }
        public decimal Weight { get; set; }
        public int PackageCount { get; set; }
        public string Description { get; set; }
        public string CustomerNotes { get; set; }
        public bool IsPaid { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal ServiceFee { get; set; }
        public decimal TotalCost { get; set; }
        public string CourierTrackingNumber { get; set; }
        public UserDto User { get; set; }
        public AddressDto SourceAddress { get; set; }
        public AddressDto DestinationAddress { get; set; }
        public List<ShipmentItemDto> Items { get; set; } = new List<ShipmentItemDto>();
        public List<ShipmentDocumentDto> Documents { get; set; } = new List<ShipmentDocumentDto>();
        public List<ShipmentStatusHistoryDto> StatusHistory { get; set; } = new List<ShipmentStatusHistoryDto>();
        public List<ShipmentNoteDto> Notes { get; set; } = new List<ShipmentNoteDto>();
        public DateTime CreatedAt { get; set; }
        public DateTime? LastModifiedAt { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Shipment, ShipmentDto>()
                .ForMember(d => d.User, opt => opt.MapFrom(s => s.User))
                .ForMember(d => d.SourceAddress, opt => opt.MapFrom(s => s.SourceAddress))
                .ForMember(d => d.DestinationAddress, opt => opt.MapFrom(s => s.DestinationAddress))
                .ForMember(d => d.Items, opt => opt.MapFrom(s => s.Items))
                .ForMember(d => d.Documents, opt => opt.MapFrom(s => s.Documents))
                .ForMember(d => d.StatusHistory, opt => opt.MapFrom(s => s.StatusHistory))
                .ForMember(d => d.Notes, opt => opt.MapFrom(s => s.Notes.Where(n => n.IsPublic)));
        }
    }
}
