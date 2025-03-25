using FastyBox.Application.Shipments.Commands.RequestShipmentPayment;
using FastyBox.Web.Settings;
using MediatR;
using Microsoft.Extensions.Options;

namespace FastyBox.Web.Services
{
    public interface IStripeService
    {
        Task<string> CreatePaymentIntentAsync(Guid shipmentId);
        string GetPublicKey();
    }

    public class StripeService : IStripeService
    {
        private readonly IMediator _mediator;
        private readonly string _publicKey;

        public StripeService(IMediator mediator, IOptions<StripeSettings> stripeSettings)
        {
            _mediator = mediator;
            _publicKey = stripeSettings.Value.PublicKey;
        }

        public async Task<string> CreatePaymentIntentAsync(Guid shipmentId)
        {
            return await _mediator.Send(new RequestShipmentPaymentCommand { ShipmentId = shipmentId });
        }

        public string GetPublicKey()
        {
            return _publicKey;
        }
    }
}
