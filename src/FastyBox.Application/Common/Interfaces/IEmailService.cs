namespace FastyBox.Application.Common.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body, bool isHtml = true, CancellationToken cancellationToken = default);
        Task SendEmailTemplateAsync(string to, string templateName, object model, CancellationToken cancellationToken = default);
    }
}
