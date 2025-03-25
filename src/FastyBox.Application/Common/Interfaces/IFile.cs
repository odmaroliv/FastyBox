namespace FastyBox.Application.Common.Interfaces
{
    public interface IFile
    {
        Task<string> SaveFileAsync(Stream content, string fileName, string contentType, CancellationToken cancellationToken = default);
        Task<Stream> GetFileAsync(string path, CancellationToken cancellationToken = default);
        Task DeleteFileAsync(string path, CancellationToken cancellationToken = default);
        string GetPublicUrl(string path);
    }
}
