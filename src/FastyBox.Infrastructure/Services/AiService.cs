using FastyBox.Application.Common.Interfaces;
using FastyBox.Infrastructure.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenAI;
using OpenAI.Chat;

namespace FastyBox.Infrastructure.Services
{
    public class AiService : IAiService
    {
        private readonly IApplicationDbContext _context;
        private readonly AiSettings _settings;
        private readonly ILogger<AiService> _logger;
        private readonly OpenAIClient _client;

        public AiService(
            IApplicationDbContext context,
            IOptions<AiSettings> settings,
            ILogger<AiService> logger)
        {
            _context = context;
            _settings = settings.Value;
            _logger = logger;

            // Initialize OpenAI client if API key is provided
            if (!string.IsNullOrEmpty(_settings.ApiKey))
            {
                _client = new OpenAIClient(_settings.ApiKey);
            }
        }

        public async Task<string> AnalyzeDocumentAsync(Stream document, CancellationToken cancellationToken = default)
        {
            // Check if AI integration is enabled
            if (_client == null)
            {
                return "AI analysis is not available.";
            }

            try
            {
                // Convert document to string (simplified - in a real application, use OCR or other document parsing)
                using var reader = new StreamReader(document);
                var text = await reader.ReadToEndAsync();

                // Create chat completion
                var chatClient = _client.GetChatClient(_settings.ModelName);

                // Create messages
                var messages = new ChatMessage[]
                {
                    ChatMessage.CreateSystemMessage("You are an assistant that analyzes shipping documents. Extract key information like product descriptions, values, weights, and any missing or inconsistent information."),
                    ChatMessage.CreateUserMessage(text)
                };

                // Complete the chat
                var options = new ChatCompletionOptions();
                options.MaxOutputTokenCount = 500;

                var response = await chatClient.CompleteChatAsync(messages, options, cancellationToken);
                return response.Value.Content.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error analyzing document with AI");
                return "Error analyzing document: " + ex.Message;
            }
        }

        public async Task<decimal> SuggestDeclaredValueAsync(string description, string url, CancellationToken cancellationToken = default)
        {
            // Check if AI integration is enabled
            if (_client == null)
            {
                return 0;
            }

            try
            {
                // Create chat completion
                var chatClient = _client.GetChatClient(_settings.ModelName);

                // Create messages
                var messages = new ChatMessage[]
                {
                    ChatMessage.CreateSystemMessage("You are an assistant that estimates product values based on descriptions and URLs. Provide only a numerical estimate in USD."),
                    ChatMessage.CreateUserMessage($"Description: {description}\nURL: {url}\n\nPlease estimate the value of this item in USD as a number only (no text).")
                };

                // Complete the chat
                var options = new ChatCompletionOptions();
                options.MaxOutputTokenCount = 50;

                var response = await chatClient.CompleteChatAsync(messages, options, cancellationToken);
                var valueText = response.Value.Content.ToString().Trim();

                // Try to extract a decimal value from the response
                if (decimal.TryParse(valueText, out var value))
                {
                    return value;
                }

                return 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error suggesting declared value with AI");
                return 0;
            }
        }

        public async Task<string> GenerateShipmentSummaryAsync(Guid shipmentId, CancellationToken cancellationToken = default)
        {
            // Check if AI integration is enabled
            if (_client == null)
            {
                return "AI summary generation is not available.";
            }

            try
            {
                // Get shipment details
                var shipment = await _context.Shipments
                    .Include(s => s.User)
                    .Include(s => s.Items)
                    .Include(s => s.StatusHistory)
                    .FirstOrDefaultAsync(s => s.Id == shipmentId, cancellationToken);

                if (shipment == null)
                {
                    return "Shipment not found.";
                }

                // Prepare shipment data as text
                var shipmentData = $@"
Tracking Number: {shipment.TrackingNumber}
Status: {shipment.Status}
Type: {shipment.Type}
Declared Value: ${shipment.DeclaredValue}
Weight: {shipment.Weight}kg
Description: {shipment.Description}
Items: {string.Join(", ", shipment.Items.Select(i => $"{i.Quantity}x {i.Name} (${i.UnitPrice})"))}
Customer: {shipment.User?.FullName}
Created: {shipment.CreatedAt}
";

                // Create chat completion
                var chatClient = _client.GetChatClient(_settings.ModelName);

                // Create messages
                var messages = new ChatMessage[]
                {
                    ChatMessage.CreateSystemMessage("You are an assistant that creates concise summaries of shipments. Highlight key details and any potential issues."),
                    ChatMessage.CreateUserMessage(shipmentData)
                };

                // Complete the chat
                var options = new ChatCompletionOptions();
                options.MaxOutputTokenCount = 500;

                var response = await chatClient.CompleteChatAsync(messages, options, cancellationToken);
                return response.Value.Content.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating shipment summary with AI for shipment {ShipmentId}", shipmentId);
                return "Error generating summary: " + ex.Message;
            }
        }
    }
}