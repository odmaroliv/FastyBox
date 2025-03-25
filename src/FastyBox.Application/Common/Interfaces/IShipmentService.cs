using FastyBox.Domain.Entities;
using FastyBox.Domain.Enums;

namespace FastyBox.Application.Common.Interfaces
{
    public interface IShipmentService
    {
        Task<Shipment> CreateShipmentAsync(Shipment shipment, CancellationToken cancellationToken = default);
        Task<Shipment> GetShipmentByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Shipment> GetShipmentByTrackingNumberAsync(string trackingNumber, CancellationToken cancellationToken = default);
        Task<IEnumerable<Shipment>> GetShipmentsByUserIdAsync(string userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Shipment>> GetShipmentsByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default);
        Task<Shipment> UpdateShipmentStatusAsync(Guid id, ShipmentStatus status, string notes = null, CancellationToken cancellationToken = default);
        Task<decimal> CalculateShippingCostAsync(Shipment shipment, CancellationToken cancellationToken = default);


    }
}
