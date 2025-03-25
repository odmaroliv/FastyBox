namespace FastyBox.Application.Common.Interfaces
{
    public interface IWebhookService
    {
        Task ProcessWebhookAsync(string payload, string signature, CancellationToken cancellationToken = default);
        Task RegisterWebhookEndpointAsync(string endpointUrl, string secret, IEnumerable<string> events, CancellationToken cancellationToken = default);
    }
}
