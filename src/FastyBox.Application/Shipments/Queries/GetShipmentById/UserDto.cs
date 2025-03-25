using AutoMapper;
using FastyBox.Application.Common.Mappings;
using FastyBox.Domain.Entities;


namespace FastyBox.Application.Shipments.Queries.GetShipmentById
{
    public class UserDto : IMapFrom<ApplicationUser>
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<ApplicationUser, UserDto>();
        }
    }
}
