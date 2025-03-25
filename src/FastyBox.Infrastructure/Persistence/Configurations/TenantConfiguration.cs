using FastyBox.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FastyBox.Infrastructure.Persistence.Configurations
{
    public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
    {
        public void Configure(EntityTypeBuilder<Tenant> builder)
        {
            builder.HasOne(t => t.ParentTenant)
                .WithMany(t => t.ChildTenants)
                .HasForeignKey(t => t.ParentTenantId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.Settings)
                .WithOne(s => s.Tenant)
                .HasForeignKey<TenantSettings>(s => s.TenantId);
        }
    }
}
