using FastyBox.Application.Common.Exceptions;
using FastyBox.Application.Common.Interfaces;
using FastyBox.Domain.Entities;
using FastyBox.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FastyBox.Application.Shipments.Commands.UploadShipmentDocument
{
    public class UploadShipmentDocumentCommand : IRequest<Guid>
    {
        public Guid ShipmentId { get; set; }
        public Stream Content { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public DocumentType Type { get; set; }
        public string Notes { get; set; }
    }

    public class UploadShipmentDocumentCommandHandler : IRequestHandler<UploadShipmentDocumentCommand, Guid>
    {
        private readonly IApplicationDbContext _context;
        private readonly IFile _fileService;
        private readonly ICurrentUserService _currentUserService;

        public UploadShipmentDocumentCommandHandler(IApplicationDbContext context, IFile fileService, ICurrentUserService currentUserService)
        {
            _context = context;
            _fileService = fileService;
            _currentUserService = currentUserService;
        }

        public async Task<Guid> Handle(UploadShipmentDocumentCommand request, CancellationToken cancellationToken)
        {
            var shipment = await _context.Shipments
                .FirstOrDefaultAsync(s => s.Id == request.ShipmentId, cancellationToken);

            if (shipment == null)
            {
                throw new NotFoundException(nameof(Shipment), request.ShipmentId);
            }

            // Upload file to storage
            var path = await _fileService.SaveFileAsync(request.Content, request.FileName, request.ContentType, cancellationToken);
            var publicUrl = _fileService.GetPublicUrl(path);

            // Create document record
            var document = new ShipmentDocument
            {
                ShipmentId = shipment.Id,
                FileName = request.FileName,
                ContentType = request.ContentType,
                StoragePath = path,
                PublicUrl = publicUrl,
                FileSize = request.Content.Length,
                Type = request.Type,
                Notes = request.Notes,
                TenantId = shipment.TenantId
            };

            _context.ShipmentDocuments.Add(document);
            await _context.SaveChangesAsync(cancellationToken);

            // If this is the first document and shipment is in draft status, update to submitted
            if (shipment.Status == ShipmentStatus.Draft)
            {
                shipment.Status = ShipmentStatus.Submitted;
                await _context.SaveChangesAsync(cancellationToken);
            }

            return document.Id;
        }
    }
}
