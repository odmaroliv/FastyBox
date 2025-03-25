using FastyBox.Application.Common.Models;
using FastyBox.Application.Shipments.Commands.CreateShipment;
using FastyBox.Application.Shipments.Commands.UpdateShipmentStatus;
using FastyBox.Application.Shipments.Commands.UploadShipmentDocument;
using FastyBox.Application.Shipments.Queries.GetAdminShipments;
using FastyBox.Application.Shipments.Queries.GetShipmentById;
using FastyBox.Application.Shipments.Queries.GetUserShipments;
using FastyBox.Domain.Entities;
using FastyBox.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Components.Forms;

namespace FastyBox.Web.Services
{
    public interface IShipmentService
    {
        Task<Guid> CreateShipmentAsync(CreateShipmentCommand command);
        Task<Shipment> GetShipmentByIdAsync(Guid id);
        Task<PaginatedList<ShipmentBriefDto>> GetUserShipmentsAsync(string userId, int pageNumber = 1, int pageSize = 10);
        Task UpdateShipmentStatusAsync(Guid shipmentId, ShipmentStatus status, string notes = null);
        Task<Guid> UploadDocumentAsync(Guid shipmentId, UploadedFile file, DocumentType documentType, string notes = null);

        Task<PaginatedList<ShipmentBriefDto>> GetAdminShipmentsAsync(
            Guid? tenantId,
            ShipmentStatus? status,
            ShipmentType? type,
            string searchString,
            int pageNumber = 1,
            int pageSize = 10);

    }

    public class ShipmentService : IShipmentService
    {
        private readonly IMediator _mediator;

        public ShipmentService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<Guid> CreateShipmentAsync(CreateShipmentCommand command)
        {
            return await _mediator.Send(command);
        }

        public async Task<Guid> UploadDocumentAsync(Guid shipmentId, UploadedFile file, DocumentType documentType, string notes = null)
        {
            using var memoryStream = new MemoryStream();
            await file.FileStream.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            var command = new UploadShipmentDocumentCommand
            {
                ShipmentId = shipmentId,
                Content = memoryStream,
                FileName = file.FileName,
                ContentType = file.ContentType,
                Type = documentType,
                Notes = notes
            };

            return await _mediator.Send(command);
        }

        public async Task<PaginatedList<ShipmentBriefDto>> GetAdminShipmentsAsync(
     Guid? tenantId,
     ShipmentStatus? status,
     ShipmentType? type,
     string searchString,
     int pageNumber = 1,
     int pageSize = 10)
        {
            return await _mediator.Send(new GetAdminShipmentsQuery
            {
                TenantId = tenantId,
                Status = status,
                Type = type,
                SearchString = searchString,
                PageNumber = pageNumber,
                PageSize = pageSize
            });
        }

        public async Task<Shipment> GetShipmentByIdAsync(Guid id)
        {
            return await _mediator.Send(new GetShipmentByIdQuery { Id = id });
        }

        public async Task<PaginatedList<ShipmentBriefDto>> GetUserShipmentsAsync(string userId, int pageNumber = 1, int pageSize = 10)
        {
            return await _mediator.Send(new GetUserShipmentsQuery
            {
                UserId = userId,
                PageNumber = pageNumber,
                PageSize = pageSize
            });
        }

        public async Task UpdateShipmentStatusAsync(Guid shipmentId, ShipmentStatus status, string notes = null)
        {
            await _mediator.Send(new UpdateShipmentStatusCommand
            {
                ShipmentId = shipmentId,
                Status = status,
                Notes = notes
            });
        }

        public async Task<Guid> UploadDocumentAsync(Guid shipmentId, IBrowserFile file, DocumentType documentType, string notes = null)
        {
            using var stream = file.OpenReadStream(10 * 1024 * 1024); // Max 10MB
            var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            var command = new UploadShipmentDocumentCommand
            {
                ShipmentId = shipmentId,
                Content = memoryStream,
                FileName = file.Name,
                ContentType = file.ContentType,
                Type = documentType,
                Notes = notes
            };

            return await _mediator.Send(command);
        }
    }

}
