using FastyBox.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using Stripe;

namespace FastyBox.Infrastructure.Services
{
    public class WebhookService : IWebhookService
    {
        private readonly IApplicationDbContext _context;
        private readonly IPaymentService _paymentService;
        private readonly ILogger<WebhookService> _logger;

        public WebhookService(
            IApplicationDbContext context,
            IPaymentService paymentService,
            ILogger<WebhookService> logger)
        {
            _context = context;
            _paymentService = paymentService;
            _logger = logger;
        }

        public async Task ProcessWebhookAsync(string payload, string signature, CancellationToken cancellationToken = default)
        {
            try
            {
                // For example, handling Stripe webhooks
                var stripeEvent = EventUtility.ConstructEvent(
                    payload, signature, "whsec_your_stripe_webhook_secret");

                // Handle the event
                switch (stripeEvent.Type)
                {
                    case "payment_intent.succeeded":
                        var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                        if (paymentIntent != null)
                        {
                            await _paymentService.ConfirmPaymentAsync(paymentIntent.Id, cancellationToken);
                        }
                        break;
                    // Handle other event types as needed
                    default:
                        _logger.LogInformation("Unhandled event type: {EventType}", stripeEvent.Type);
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing webhook");
                throw;
            }
        }

        public async Task RegisterWebhookEndpointAsync(string endpointUrl, string secret, IEnumerable<string> events, CancellationToken cancellationToken = default)
        {
            // This is a simplified example for demonstration
            // In a real application, this would register webhooks with external services
            _logger.LogInformation("Registering webhook endpoint {EndpointUrl} for events {@Events}", endpointUrl, events);

            // Record the webhook registration somewhere if needed
            await Task.CompletedTask;
        }
    }

}
