using FastyBox.Application.Common.Interfaces;
using FastyBox.Infrastructure.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stripe;

namespace FastyBox.Infrastructure.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<PaymentService> _logger;
        private readonly StripeSettings _settings;

        public PaymentService(
            IApplicationDbContext context,
            IOptions<StripeSettings> settings,
            ILogger<PaymentService> logger)
        {
            _context = context;
            _logger = logger;
            _settings = settings.Value;

            // Configure Stripe API key
            StripeConfiguration.ApiKey = _settings.ApiKey;
        }

        public async Task<string> CreatePaymentIntentAsync(Guid shipmentId, CancellationToken cancellationToken = default)
        {
            var shipment = await _context.Shipments.FindAsync(new object[] { shipmentId }, cancellationToken);
            if (shipment == null)
            {
                throw new ArgumentException($"Shipment with ID {shipmentId} not found", nameof(shipmentId));
            }

            if (shipment.IsPaid)
            {
                throw new InvalidOperationException($"Shipment {shipmentId} is already paid");
            }

            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(shipment.TotalCost * 100), // Convert to cents
                Currency = "mxn",
                Description = $"Payment for shipment {shipment.TrackingNumber}",
                Metadata = new Dictionary<string, string>
            {
                { "ShipmentId", shipmentId.ToString() },
                { "TrackingNumber", shipment.TrackingNumber }
            }
            };

            try
            {
                var service = new PaymentIntentService();
                var intent = await service.CreateAsync(options, null, cancellationToken);

                // Update shipment with payment intent ID
                shipment.PaymentIntentId = intent.Id;
                await _context.SaveChangesAsync(cancellationToken);

                return intent.ClientSecret;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating payment intent for shipment {ShipmentId}", shipmentId);
                throw;
            }
        }

        public async Task<bool> ConfirmPaymentAsync(string paymentIntentId, CancellationToken cancellationToken = default)
        {
            try
            {
                var service = new PaymentIntentService();
                var intent = await service.GetAsync(paymentIntentId, null, null, cancellationToken);

                if (intent.Status == "succeeded")
                {
                    // Find associated shipment
                    if (intent.Metadata.TryGetValue("ShipmentId", out var shipmentIdStr) &&
                        Guid.TryParse(shipmentIdStr, out var shipmentId))
                    {
                        var shipment = await _context.Shipments.FindAsync(new object[] { shipmentId }, cancellationToken);
                        if (shipment != null)
                        {
                            shipment.IsPaid = true;
                            await _context.SaveChangesAsync(cancellationToken);
                        }
                    }

                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error confirming payment for intent {PaymentIntentId}", paymentIntentId);
                throw;
            }
        }

        public async Task<string> GetPaymentStatusAsync(string paymentIntentId, CancellationToken cancellationToken = default)
        {
            try
            {
                var service = new PaymentIntentService();
                var intent = await service.GetAsync(paymentIntentId, null, null, cancellationToken);
                return intent.Status;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting payment status for intent {PaymentIntentId}", paymentIntentId);
                throw;
            }
        }
    }

}
