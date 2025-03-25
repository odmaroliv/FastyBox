using FastyBox.Application.Common.Exceptions;
using FastyBox.Application.Common.Interfaces;
using FastyBox.Domain.Entities;
using FastyBox.Domain.Enums;
using MediatR;

namespace FastyBox.Application.Shipments.Commands.RequestShipmentPayment
{
    public class RequestShipmentPaymentCommand : IRequest<string>
    {
        public Guid ShipmentId { get; set; }
    }

    public class RequestShipmentPaymentCommandHandler : IRequestHandler<RequestShipmentPaymentCommand, string>
    {
        private readonly IApplicationDbContext _context;
        private readonly IShipmentService _shipmentService;
        private readonly IPaymentService _paymentService;
        private readonly INotificationService _notificationService;

        public RequestShipmentPaymentCommandHandler(
            IApplicationDbContext context,
            IShipmentService shipmentService,
            IPaymentService paymentService,
            INotificationService notificationService)
        {
            _context = context;
            _shipmentService = shipmentService;
            _paymentService = paymentService;
            _notificationService = notificationService;
        }

        public async Task<string> Handle(RequestShipmentPaymentCommand request, CancellationToken cancellationToken)
        {
            var shipment = await _shipmentService.GetShipmentByIdAsync(request.ShipmentId, cancellationToken);
            if (shipment == null)
            {
                throw new NotFoundException(nameof(Shipment), request.ShipmentId);
            }

            if (shipment.IsPaid)
            {
                throw new InvalidOperationException("Shipment is already paid");
            }

            // Calculate shipping cost if not already done
            if (shipment.TotalCost <= 0)
            {
                await _shipmentService.CalculateShippingCostAsync(shipment, cancellationToken);
            }

            // Update shipment status
            await _shipmentService.UpdateShipmentStatusAsync(shipment.Id, ShipmentStatus.AwaitingPayment, "Payment requested", cancellationToken);

            // Create payment intent
            var clientSecret = await _paymentService.CreatePaymentIntentAsync(shipment.Id, cancellationToken);

            // Send notification to user
            await _notificationService.CreateNotificationAsync(
                shipment.UserId,
                "Payment Required",
                $"Please complete payment for your shipment #{shipment.TrackingNumber}.",
                NotificationType.PaymentRequired,
                $"/shipments/{shipment.Id}/payment",
                shipment.Id,
                cancellationToken);

            return clientSecret;
        }
    }
}
