using FastyBox.Domain.Common;

namespace FastyBox.Domain.Entities
{
    public class PricingRule : BaseTenantEntity
    {
        public decimal MinWeight { get; set; }
        public decimal MaxWeight { get; set; }
        public decimal Price { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
