using AutoMapper;
using FastyBox.Application.Common.Mappings;
using FastyBox.Domain.Entities;

namespace FastyBox.Application.Shipments.Queries.GetShipmentById
{
    public class ShipmentItemDto : IMapFrom<ShipmentItem>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal Weight { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<ShipmentItem, ShipmentItemDto>();
        }
    }
}
