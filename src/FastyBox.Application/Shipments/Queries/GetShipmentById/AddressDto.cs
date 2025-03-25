using AutoMapper;
using FastyBox.Application.Common.Mappings;
using FastyBox.Domain.Entities;


namespace FastyBox.Application.Shipments.Queries.GetShipmentById
{
    public class AddressDto : IMapFrom<Address>
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsDefault { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<Address, AddressDto>();
        }
    }
}
