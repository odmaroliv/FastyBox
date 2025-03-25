using FastyBox.Application.Common.Exceptions;
using FastyBox.Application.Common.Interfaces;
using FastyBox.Domain.Entities;
using FastyBox.Domain.Enums;
using MediatR;

namespace FastyBox.Application.Shipments.Commands.UpdateShipmentStatus
{
    public class UpdateShipmentStatusCommand : IRequest
    {
        public Guid ShipmentId { get; set; }
        public ShipmentStatus Status { get; set; }
        public string Notes { get; set; }
    }

    public class UpdateShipmentStatusCommandHandler : IRequestHandler<UpdateShipmentStatusCommand>
    {
        private readonly IShipmentService _shipmentService;
        private readonly INotificationService _notificationService;

        public UpdateShipmentStatusCommandHandler(IShipmentService shipmentService, INotificationService notificationService)
        {
            _shipmentService = shipmentService;
            _notificationService = notificationService;
        }

        public async Task Handle(UpdateShipmentStatusCommand request, CancellationToken cancellationToken)
        {
            var shipment = await _shipmentService.GetShipmentByIdAsync(request.ShipmentId, cancellationToken);
            if (shipment == null)
            {
                throw new NotFoundException(nameof(Shipment), request.ShipmentId);
            }

            await _shipmentService.UpdateShipmentStatusAsync(request.ShipmentId, request.Status, request.Notes, cancellationToken);
        }
    }
}
