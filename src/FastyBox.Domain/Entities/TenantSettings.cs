using FastyBox.Domain.Common;

namespace FastyBox.Domain.Entities
{
    public class TenantSettings : BaseEntity
    {
        public Guid TenantId { get; set; }
        public virtual Tenant Tenant { get; set; }
        public decimal ProfitMarginPercentage { get; set; } = 30.0m; // Default 30%
        public string StripeApiKey { get; set; }
        public string StripeWebhookSecret { get; set; }
        public string UsAddress { get; set; }
        public bool EnableAiIntegration { get; set; }
        public string OpenAiApiKey { get; set; }
    }
}
