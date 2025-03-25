namespace FastyBox.Application.Common.Interfaces
{
    public interface IAiService
    {
        Task<string> AnalyzeDocumentAsync(Stream document, CancellationToken cancellationToken = default);
        Task<decimal> SuggestDeclaredValueAsync(string description, string url, CancellationToken cancellationToken = default);
        Task<string> GenerateShipmentSummaryAsync(Guid shipmentId, CancellationToken cancellationToken = default);
    }
}
