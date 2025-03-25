using FastyBox.Application.Common.Interfaces;
using FastyBox.Infrastructure.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace FastyBox.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly SendGridClient _client;
        private readonly EmailSettings _settings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<EmailSettings> settings, ILogger<EmailService> logger)
        {
            _settings = settings.Value;
            _logger = logger;
            _client = new SendGridClient(_settings.ApiKey);
        }

        public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = true, CancellationToken cancellationToken = default)
        {
            try
            {
                var from = new EmailAddress(_settings.FromEmail, _settings.FromName);
                var toAddress = new EmailAddress(to);
                var msg = MailHelper.CreateSingleEmail(from, toAddress, subject, isHtml ? null : body, isHtml ? body : null);
                var response = await _client.SendEmailAsync(msg, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Failed to send email: {StatusCode}", response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while sending email");
                throw;
            }
        }

        public async Task SendEmailTemplateAsync(string to, string templateName, object model, CancellationToken cancellationToken = default)
        {
            // Implementation will depend on template engine used
            // This is a simplified example using SendGrid templates
            var templateId = templateName switch
            {
                "WelcomeEmail" => _settings.WelcomeEmailTemplateId,
                "ShipmentStatusUpdate" => _settings.ShipmentStatusUpdateTemplateId,
                "DocumentRequest" => _settings.DocumentRequestTemplateId,
                "PaymentRequired" => _settings.PaymentRequiredTemplateId,
                _ => throw new ArgumentException($"Template '{templateName}' not found")
            };

            try
            {
                var from = new EmailAddress(_settings.FromEmail, _settings.FromName);
                var toAddress = new EmailAddress(to);
                var msg = MailHelper.CreateSingleTemplateEmail(from, toAddress, templateId, model);
                var response = await _client.SendEmailAsync(msg, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Failed to send template email: {StatusCode}", response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while sending template email");
                throw;
            }
        }
    }
}
