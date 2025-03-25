using AutoMapper;
using FastyBox.Application.Common.Mappings;
using FastyBox.Domain.Entities;
using FastyBox.Domain.Enums;

namespace FastyBox.Application.Shipments.Queries.GetUserShipments
{
    public class ShipmentBriefDto : IMapFrom<Shipment>
    {
        public Guid Id { get; set; }
        public string TrackingNumber { get; set; }
        public ShipmentType Type { get; set; }
        public ShipmentStatus Status { get; set; }
        public decimal DeclaredValue { get; set; }
        public decimal Weight { get; set; }
        public string Description { get; set; }
        public bool IsPaid { get; set; }
        public decimal TotalCost { get; set; }
        public string CourierTrackingNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastModifiedAt { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public UserBriefDto User { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Shipment, ShipmentBriefDto>();
        }
    }
    public class UserBriefDto
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
    }
}
