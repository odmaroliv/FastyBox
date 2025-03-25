using FastyBox.Application.Common.Interfaces;
using FastyBox.Domain.Entities;
using FastyBox.Infrastructure.Persistence;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;

namespace FastyBox.Web.Services
{
    public interface IDocumentService
    {
        Task<Guid> UploadDocumentAsync(ShipmentDocument document, IBrowserFile file);
        Task<IEnumerable<ShipmentDocument>> GetDocumentsForShipmentAsync(Guid shipmentId);
        Task<ShipmentDocument> GetDocumentByIdAsync(Guid documentId);
        Task DeleteDocumentAsync(Guid documentId);
    }

    public class DocumentService : IDocumentService
    {
        private readonly ApplicationDbContext _context;
        private readonly IFile _fileService;

        public DocumentService(ApplicationDbContext context, IFile fileService)
        {
            _context = context;
            _fileService = fileService;
        }

        public async Task<Guid> UploadDocumentAsync(ShipmentDocument document, IBrowserFile file)
        {
            using var stream = file.OpenReadStream(10 * 1024 * 1024); // Max 10MB

            // Save to storage
            var path = await _fileService.SaveFileAsync(stream, file.Name, file.ContentType);
            var publicUrl = _fileService.GetPublicUrl(path);

            // Update document properties
            document.FileName = file.Name;
            document.ContentType = file.ContentType;
            document.StoragePath = path;
            document.PublicUrl = publicUrl;
            document.FileSize = file.Size;

            // Save to database
            _context.ShipmentDocuments.Add(document);
            await _context.SaveChangesAsync();

            return document.Id;
        }

        public async Task<IEnumerable<ShipmentDocument>> GetDocumentsForShipmentAsync(Guid shipmentId)
        {
            return await _context.ShipmentDocuments
                .Where(d => d.ShipmentId == shipmentId)
                .OrderByDescending(d => d.CreatedAt)
                .ToListAsync();
        }

        public async Task<ShipmentDocument> GetDocumentByIdAsync(Guid documentId)
        {
            return await _context.ShipmentDocuments.FindAsync(documentId);
        }

        public async Task DeleteDocumentAsync(Guid documentId)
        {
            var document = await _context.ShipmentDocuments.FindAsync(documentId);
            if (document != null)
            {
                // Delete from storage
                await _fileService.DeleteFileAsync(document.StoragePath);

                // Delete from database
                _context.ShipmentDocuments.Remove(document);
                await _context.SaveChangesAsync();
            }
        }
    }

}
