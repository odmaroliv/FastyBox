using FastyBox.Domain.Entities;

namespace FastyBox.Domain.Common
{
    public abstract class BaseTenantEntity : BaseEntity
    {
        public Guid TenantId { get; set; }
        public virtual Tenant Tenant { get; set; }
    }
}
