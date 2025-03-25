using AutoMapper;
using FastyBox.Application.Common.Mappings;
using FastyBox.Domain.Entities;
using FastyBox.Domain.Enums;

namespace FastyBox.Application.Shipments.Queries.GetShipmentById
{
    public class ShipmentStatusHistoryDto : IMapFrom<ShipmentStatusHistory>
    {
        public Guid Id { get; set; }
        public ShipmentStatus PreviousStatus { get; set; }
        public ShipmentStatus NewStatus { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<ShipmentStatusHistory, ShipmentStatusHistoryDto>();
        }
    }
}
