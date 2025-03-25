namespace FastyBox.Application.Common.Interfaces
{
    public interface IPaymentService
    {
        Task<string> CreatePaymentIntentAsync(Guid shipmentId, CancellationToken cancellationToken = default);
        Task<bool> ConfirmPaymentAsync(string paymentIntentId, CancellationToken cancellationToken = default);
        Task<string> GetPaymentStatusAsync(string paymentIntentId, CancellationToken cancellationToken = default);
    }
}
