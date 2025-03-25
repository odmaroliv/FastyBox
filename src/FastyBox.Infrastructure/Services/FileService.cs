using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using FastyBox.Application.Common.Interfaces;
using FastyBox.Infrastructure.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FastyBox.Infrastructure.Services
{
    public class FileService : IFile
    {
        private readonly StorageSettings _settings;
        private readonly ILogger<FileService> _logger;
        private readonly BlobServiceClient _blobServiceClient;

        public FileService(IOptions<StorageSettings> settings, ILogger<FileService> logger)
        {
            _settings = settings.Value;
            _logger = logger;
            _blobServiceClient = new BlobServiceClient(_settings.ConnectionString);
        }

        public async Task<string> SaveFileAsync(Stream content, string fileName, string contentType, CancellationToken cancellationToken = default)
        {
            try
            {
                var blobContainerClient = _blobServiceClient.GetBlobContainerClient(_settings.ContainerName);
                await blobContainerClient.CreateIfNotExistsAsync(PublicAccessType.Blob, null, cancellationToken);

                // Generate a unique file name to avoid collisions
                var uniqueFileName = $"{Guid.NewGuid()}-{fileName}";
                var blobClient = blobContainerClient.GetBlobClient(uniqueFileName);

                var blobHttpHeaders = new BlobHttpHeaders
                {
                    ContentType = contentType
                };

                await blobClient.UploadAsync(content, new BlobUploadOptions { HttpHeaders = blobHttpHeaders }, cancellationToken);

                return uniqueFileName;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving file {FileName}", fileName);
                throw;
            }
        }

        public async Task<Stream> GetFileAsync(string path, CancellationToken cancellationToken = default)
        {
            try
            {
                var blobContainerClient = _blobServiceClient.GetBlobContainerClient(_settings.ContainerName);
                var blobClient = blobContainerClient.GetBlobClient(path);

                var downloadInfo = await blobClient.DownloadAsync(cancellationToken);
                return downloadInfo.Value.Content;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting file {Path}", path);
                throw;
            }
        }

        public async Task DeleteFileAsync(string path, CancellationToken cancellationToken = default)
        {
            try
            {
                var blobContainerClient = _blobServiceClient.GetBlobContainerClient(_settings.ContainerName);
                var blobClient = blobContainerClient.GetBlobClient(path);

                await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting file {Path}", path);
                throw;
            }
        }

        public string GetPublicUrl(string path)
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(_settings.ContainerName);
            var blobClient = blobContainerClient.GetBlobClient(path);
            return blobClient.Uri.ToString();
        }
    }

}
