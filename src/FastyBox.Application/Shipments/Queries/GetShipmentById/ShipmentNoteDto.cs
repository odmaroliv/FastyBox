using AutoMapper;
using FastyBox.Application.Common.Mappings;
using FastyBox.Domain.Entities;

namespace FastyBox.Application.Shipments.Queries.GetShipmentById
{
    public class ShipmentNoteDto : IMapFrom<ShipmentNote>
    {
        public Guid Id { get; set; }
        public string Note { get; set; }
        public bool IsSystemGenerated { get; set; }
        public bool IsPublic { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public UserDto User { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<ShipmentNote, ShipmentNoteDto>()
                .ForMember(d => d.User, opt => opt.MapFrom(s => s.User));
        }
    }
}
