using FastyBox.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FastyBox.Infrastructure.Persistence.Configurations
{
    public class ShipmentConfiguration : IEntityTypeConfiguration<Shipment>
    {
        public void Configure(EntityTypeBuilder<Shipment> builder)
        {
            builder.HasOne(s => s.User)
                .WithMany(u => u.Shipments)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.Tenant)
                .WithMany(t => t.Shipments)
                .HasForeignKey(s => s.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.SourceAddress)
                .WithMany()
                .HasForeignKey(s => s.SourceAddressId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.DestinationAddress)
                .WithMany()
                .HasForeignKey(s => s.DestinationAddressId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
