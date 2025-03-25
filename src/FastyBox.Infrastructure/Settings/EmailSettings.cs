namespace FastyBox.Infrastructure.Settings
{
    public class EmailSettings
    {
        public string ApiKey { get; set; }
        public string FromEmail { get; set; }
        public string FromName { get; set; }
        public string WelcomeEmailTemplateId { get; set; }
        public string ShipmentStatusUpdateTemplateId { get; set; }
        public string DocumentRequestTemplateId { get; set; }
        public string PaymentRequiredTemplateId { get; set; }
    }
}
