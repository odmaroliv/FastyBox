using FastyBox.Domain.Entities;
using FastyBox.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FastyBox.Web.Services
{
    public interface IPricingService
    {
        Task<IEnumerable<PricingRule>> GetPricingRulesAsync(Guid tenantId);
        Task<PricingRule> GetPricingRuleForWeightAsync(Guid tenantId, decimal weight);
        Task<decimal> CalculateShippingCostAsync(Guid tenantId, decimal weight);
        Task<decimal> CalculateServiceFeeAsync(Guid tenantId, decimal declaredValue);
        Task<decimal> CalculateTotalCostAsync(Guid tenantId, decimal declaredValue, decimal weight);
    }

    public class PricingService : IPricingService
    {
        private readonly ApplicationDbContext _context;

        public PricingService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PricingRule>> GetPricingRulesAsync(Guid tenantId)
        {
            return await _context.PricingRules
                .Where(pr => pr.TenantId == tenantId && pr.IsActive)
                .OrderBy(pr => pr.MinWeight)
                .ToListAsync();
        }

        public async Task<PricingRule> GetPricingRuleForWeightAsync(Guid tenantId, decimal weight)
        {
            return await _context.PricingRules
                .Where(pr => pr.TenantId == tenantId && pr.IsActive)
                .Where(pr => weight >= pr.MinWeight && weight <= pr.MaxWeight)
                .FirstOrDefaultAsync();
        }

        public async Task<decimal> CalculateShippingCostAsync(Guid tenantId, decimal weight)
        {
            var pricingRule = await GetPricingRuleForWeightAsync(tenantId, weight);
            return pricingRule?.Price ?? 0;
        }

        public async Task<decimal> CalculateServiceFeeAsync(Guid tenantId, decimal declaredValue)
        {
            var tenantSettings = await _context.TenantSettings
                .FirstOrDefaultAsync(ts => ts.TenantId == tenantId);

            if (tenantSettings == null)
            {
                throw new InvalidOperationException($"Tenant settings not found for tenant ID {tenantId}");
            }

            return declaredValue * (tenantSettings.ProfitMarginPercentage / 100);
        }

        public async Task<decimal> CalculateTotalCostAsync(Guid tenantId, decimal declaredValue, decimal weight)
        {
            var shippingCost = await CalculateShippingCostAsync(tenantId, weight);
            var serviceFee = await CalculateServiceFeeAsync(tenantId, declaredValue);

            return shippingCost + serviceFee;
        }
    }

}
