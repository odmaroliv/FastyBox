using FastyBox.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace FastyBox.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        public Guid? TenantId { get; set; }
        public virtual Tenant Tenant { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public UserType UserType { get; set; }
        public bool IsActive { get; set; } = true;
        public virtual Address DefaultAddress { get; set; }
        public virtual ICollection<Shipment> Shipments { get; set; } = new List<Shipment>();
    }
}
