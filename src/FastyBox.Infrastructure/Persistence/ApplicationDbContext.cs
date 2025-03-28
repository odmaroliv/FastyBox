﻿using FastyBox.Application.Common.Interfaces;
using FastyBox.Domain.Common;
using FastyBox.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;

namespace FastyBox.Infrastructure.Persistence
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IApplicationDbContext
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IDateTime _dateTime;

        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options,
            ICurrentUserService currentUserService,
            IDateTime dateTime) : base(options)
        {
            _currentUserService = currentUserService;
            _dateTime = dateTime;
        }

        // Entity DbSets
        public DbSet<Tenant> Tenants => Set<Tenant>();
        public DbSet<TenantSettings> TenantSettings => Set<TenantSettings>();
        public DbSet<Address> Addresses => Set<Address>();
        public DbSet<Shipment> Shipments => Set<Shipment>();
        public DbSet<ShipmentItem> ShipmentItems => Set<ShipmentItem>();
        public DbSet<ShipmentDocument> ShipmentDocuments => Set<ShipmentDocument>();
        public DbSet<ShipmentStatusHistory> ShipmentStatusHistories => Set<ShipmentStatusHistory>();
        public DbSet<ShipmentNote> ShipmentNotes => Set<ShipmentNote>();
        public DbSet<PricingRule> PricingRules => Set<PricingRule>();
        public DbSet<Notification> Notifications => Set<Notification>();

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.Id = entry.Entity.Id == Guid.Empty ? Guid.NewGuid() : entry.Entity.Id;
                        entry.Entity.CreatedBy = _currentUserService.UserId;
                        entry.Entity.CreatedAt = _dateTime.UtcNow;
                        break;

                    case EntityState.Modified:
                        entry.Entity.LastModifiedBy = _currentUserService.UserId;
                        entry.Entity.LastModifiedAt = _dateTime.UtcNow;
                        break;

                    case EntityState.Deleted:
                        entry.State = EntityState.Modified;
                        entry.Entity.IsDeleted = true;
                        entry.Entity.LastModifiedBy = _currentUserService.UserId;
                        entry.Entity.LastModifiedAt = _dateTime.UtcNow;
                        break;
                }
            }

            var result = await base.SaveChangesAsync(cancellationToken);
            return result;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Apply entity configurations from assembly
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            // Las relaciones específicas ahora están en las clases de configuración
            // Solamente definimos relaciones que no están en clases de configuración aquí
            ConfigureAdditionalRelationships(builder);

            // Apply global query filter for soft delete
            ApplySoftDeleteFilter(builder);

            // Apply global query filter for multi-tenancy
            ApplyMultiTenancyFilter(builder);
        }

        private void ConfigureAdditionalRelationships(ModelBuilder builder)
        {
            // Estas son relaciones que no están definidas en las clases de configuración

            // Shipment items, documents, status history, notes
            builder.Entity<Shipment>()
                .HasMany(s => s.Items)
                .WithOne(i => i.Shipment)
                .HasForeignKey(i => i.ShipmentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Shipment>()
                .HasMany(s => s.Documents)
                .WithOne(d => d.Shipment)
                .HasForeignKey(d => d.ShipmentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Shipment>()
                .HasMany(s => s.StatusHistory)
                .WithOne(sh => sh.Shipment)
                .HasForeignKey(sh => sh.ShipmentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Shipment>()
                .HasMany(s => s.Notes)
                .WithOne(n => n.Shipment)
                .HasForeignKey(n => n.ShipmentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Address relationship
            builder.Entity<Address>()
                .HasOne<ApplicationUser>(a => a.User)
                .WithOne(u => u.DefaultAddress)
                .HasForeignKey<Address>(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        private void ApplySoftDeleteFilter(ModelBuilder builder)
        {
            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                // Check if the entity inherits from BaseEntity
                if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
                {
                    var parameter = Expression.Parameter(entityType.ClrType, "e");
                    var propertyMethodInfo = typeof(EF).GetMethod(nameof(EF.Property))?.MakeGenericMethod(typeof(bool));
                    var isDeletedProperty = Expression.Call(propertyMethodInfo!, parameter, Expression.Constant("IsDeleted"));
                    var compareExpression = Expression.Equal(isDeletedProperty, Expression.Constant(false));
                    var lambda = Expression.Lambda(compareExpression, parameter);

                    builder.Entity(entityType.ClrType).HasQueryFilter(lambda);
                }
            }
        }

        private void ApplyMultiTenancyFilter(ModelBuilder builder)
        {
            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                // Check if the entity inherits from BaseTenantEntity
                if (typeof(BaseTenantEntity).IsAssignableFrom(entityType.ClrType))
                {
                    var method = SetGlobalQueryMethod.MakeGenericMethod(entityType.ClrType);
                    method.Invoke(this, new object[] { builder });
                }
            }
        }

        private static readonly MethodInfo SetGlobalQueryMethod = typeof(ApplicationDbContext)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Single(t => t.IsGenericMethod && t.Name == "SetGlobalQuery");

        public void SetGlobalQuery<T>(ModelBuilder builder) where T : BaseTenantEntity
        {
            builder.Entity<T>().HasQueryFilter(e => !e.IsDeleted &&
                (_currentUserService.TenantId == null || e.TenantId == _currentUserService.TenantId));
        }
    }
}