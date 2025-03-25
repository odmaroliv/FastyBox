using FastyBox.Domain.Common;

namespace FastyBox.Domain.Entities
{
    public class Tenant : BaseEntity
    {
        public string Name { get; set; }
        public string? Slug { get; set; }
        public string? LogoUrl { get; set; }
        public string PrimaryColor { get; set; }
        public string SecondaryColor { get; set; }
        public bool IsActive { get; set; }
        public Guid? ParentTenantId { get; set; }
        public virtual Tenant ParentTenant { get; set; }

        public virtual ICollection<Tenant> ChildTenants { get; set; } = new List<Tenant>();

        public virtual ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
        public virtual ICollection<Shipment> Shipments { get; set; } = new List<Shipment>();
        public virtual TenantSettings Settings { get; set; }
    }

}
