namespace FastyBox.Application.Common.Models
{
    public class UploadedFile
    {
        public Stream FileStream { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public long Size { get; set; }
    }


}
