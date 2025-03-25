using AutoMapper;
using FastyBox.Application.Common.Interfaces;
using FastyBox.Domain.Entities;
using FastyBox.Domain.Enums;
using MediatR;

namespace FastyBox.Application.Shipments.Commands.CreateShipment
{
    public class CreateShipmentCommand : IRequest<Guid>
    {
        public string UserId { get; set; }
        public Guid TenantId { get; set; }
        public ShipmentType Type { get; set; }
        public string Description { get; set; }
        public decimal DeclaredValue { get; set; }
        public decimal Weight { get; set; }
        public Guid? SourceAddressId { get; set; }
        public Guid? DestinationAddressId { get; set; }
        public List<ShipmentItemDto> Items { get; set; } = new List<ShipmentItemDto>();
    }

    public class ShipmentItemDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal Weight { get; set; }

    }

    public class CreateShipmentCommandHandler : IRequestHandler<CreateShipmentCommand, Guid>
    {
        private readonly IApplicationDbContext _context;
        private readonly IShipmentService _shipmentService;
        private readonly IMapper _mapper;

        public CreateShipmentCommandHandler(IApplicationDbContext context, IShipmentService shipmentService, IMapper mapper)
        {
            _context = context;
            _shipmentService = shipmentService;
            _mapper = mapper;
        }

        public async Task<Guid> Handle(CreateShipmentCommand request, CancellationToken cancellationToken)
        {
            var shipment = new Shipment
            {
                UserId = request.UserId,
                TenantId = request.TenantId,
                Type = request.Type,
                Description = request.Description,
                DeclaredValue = request.DeclaredValue,
                Weight = request.Weight,
                SourceAddressId = request.SourceAddressId,
                DestinationAddressId = request.DestinationAddressId,
                Status = ShipmentStatus.Draft
            };

            foreach (var itemDto in request.Items)
            {
                var item = new ShipmentItem
                {
                    Name = itemDto.Name,
                    Description = itemDto.Description,
                    Url = itemDto.Url,
                    Quantity = itemDto.Quantity,
                    UnitPrice = itemDto.UnitPrice,
                    TotalPrice = itemDto.UnitPrice * itemDto.Quantity,
                    Weight = itemDto.Weight,
                    TenantId = request.TenantId
                };

                shipment.Items.Add(item);
            }

            // Calculate total weight based on items if not provided
            if (shipment.Weight <= 0 && shipment.Items.Count > 0)
            {
                shipment.Weight = shipment.Items.Sum(i => i.Weight * i.Quantity);
            }

            var createdShipment = await _shipmentService.CreateShipmentAsync(shipment, cancellationToken);
            return createdShipment.Id;
        }
    }

}
