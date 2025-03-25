using FastyBox.Application.Common.Interfaces;
using FastyBox.Domain.Entities;
using FastyBox.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace FastyBox.Infrastructure.Services
{
    public class ShipmentService : IShipmentService
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly IDateTime _dateTime;
        private readonly INotificationService _notificationService;

        public ShipmentService(
            IApplicationDbContext context,
            ICurrentUserService currentUserService,
            IDateTime dateTime,
            INotificationService notificationService)
        {
            _context = context;
            _currentUserService = currentUserService;
            _dateTime = dateTime;
            _notificationService = notificationService;
        }

        public async Task<Shipment> CreateShipmentAsync(Shipment shipment, CancellationToken cancellationToken = default)
        {
            // Generate tracking number
            shipment.TrackingNumber = GenerateTrackingNumber();

            // Initialize status
            shipment.Status = ShipmentStatus.Draft;

            // Add to context
            _context.Shipments.Add(shipment);

            // Create initial status history entry
            var statusHistory = new ShipmentStatusHistory
            {
                ShipmentId = shipment.Id,
                PreviousStatus = ShipmentStatus.Draft,
                NewStatus = ShipmentStatus.Draft,
                Notes = "Shipment created",
                TenantId = shipment.TenantId
            };

            _context.ShipmentStatusHistories.Add(statusHistory);

            await _context.SaveChangesAsync(cancellationToken);

            return shipment;
        }

        public async Task<Shipment> GetShipmentByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Shipments
                .Include(s => s.User)
                .Include(s => s.SourceAddress)
                .Include(s => s.DestinationAddress)
                .Include(s => s.Items)
                .Include(s => s.Documents)
                .Include(s => s.StatusHistory)
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        }

        public async Task<Shipment> GetShipmentByTrackingNumberAsync(string trackingNumber, CancellationToken cancellationToken = default)
        {
            return await _context.Shipments
                .Include(s => s.User)
                .Include(s => s.SourceAddress)
                .Include(s => s.DestinationAddress)
                .Include(s => s.Items)
                .Include(s => s.Documents)
                .Include(s => s.StatusHistory)
                .FirstOrDefaultAsync(s => s.TrackingNumber == trackingNumber, cancellationToken);
        }

        public async Task<IEnumerable<Shipment>> GetShipmentsByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            return await _context.Shipments
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Shipment>> GetShipmentsByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default)
        {
            return await _context.Shipments
                .Where(s => s.TenantId == tenantId)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<Shipment> UpdateShipmentStatusAsync(Guid id, ShipmentStatus status, string notes = null, CancellationToken cancellationToken = default)
        {
            var shipment = await _context.Shipments.FindAsync(new object[] { id }, cancellationToken);
            if (shipment == null)
            {
                throw new ArgumentException($"Shipment with ID {id} not found", nameof(id));
            }

            var previousStatus = shipment.Status;
            shipment.Status = status;

            // Create status history entry
            var statusHistory = new ShipmentStatusHistory
            {
                ShipmentId = shipment.Id,
                PreviousStatus = previousStatus,
                NewStatus = status,
                Notes = notes,
                TenantId = shipment.TenantId
            };

            _context.ShipmentStatusHistories.Add(statusHistory);
            await _context.SaveChangesAsync(cancellationToken);

            // Create notification for user
            await _notificationService.CreateNotificationAsync(
                shipment.UserId,
                "Shipment Status Updated",
                $"Your shipment #{shipment.TrackingNumber} status has been updated to {status}",
                NotificationType.ShipmentStatusUpdate,
                $"/shipments/{shipment.Id}",
                shipment.Id,
                cancellationToken);

            return shipment;
        }

        public async Task<decimal> CalculateShippingCostAsync(Shipment shipment, CancellationToken cancellationToken = default)
        {
            // Get tenant settings for profit margin percentage
            var tenantSettings = await _context.TenantSettings
                .FirstOrDefaultAsync(ts => ts.TenantId == shipment.TenantId, cancellationToken);

            if (tenantSettings == null)
            {
                throw new InvalidOperationException($"Tenant settings not found for tenant ID {shipment.TenantId}");
            }

            // Calculate service fee based on declared value and profit margin
            var serviceFee = shipment.DeclaredValue * (tenantSettings.ProfitMarginPercentage / 100);

            // Find pricing rule for shipping based on weight
            var pricingRule = await _context.PricingRules
                .Where(pr => pr.TenantId == shipment.TenantId && pr.IsActive)
                .Where(pr => shipment.Weight >= pr.MinWeight && shipment.Weight <= pr.MaxWeight)
                .FirstOrDefaultAsync(cancellationToken);

            if (pricingRule == null)
            {
                throw new InvalidOperationException($"No pricing rule found for weight {shipment.Weight}kg");
            }

            // Update shipment with calculated costs
            shipment.ServiceFee = serviceFee;
            shipment.ShippingCost = pricingRule.Price;
            shipment.TotalCost = serviceFee + pricingRule.Price;

            await _context.SaveChangesAsync(cancellationToken);

            return shipment.TotalCost;
        }

        private string GenerateTrackingNumber()
        {
            // Format: FBX-YYYYMMDD-XXXX (where XXXX is a random number)
            var dateStr = _dateTime.UtcNow.ToString("yyyyMMdd");
            var random = new Random();
            var randomPart = random.Next(1000, 10000).ToString();

            return $"FBX-{dateStr}-{randomPart}";
        }
    }
}
