using FastyBox.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FastyBox.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<ApplicationUser> Users { get; }
        DbSet<Tenant> Tenants { get; }
        DbSet<TenantSettings> TenantSettings { get; }
        DbSet<Address> Addresses { get; }
        DbSet<Shipment> Shipments { get; }
        DbSet<ShipmentItem> ShipmentItems { get; }
        DbSet<ShipmentDocument> ShipmentDocuments { get; }
        DbSet<ShipmentStatusHistory> ShipmentStatusHistories { get; }
        DbSet<ShipmentNote> ShipmentNotes { get; }
        DbSet<PricingRule> PricingRules { get; }
        DbSet<Notification> Notifications { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
