namespace FileService.Models
{
    public class FileServiceOptions
    {
        public string StoragePath { get; set; } = "uploads";
        public string BaseUrl { get; set; } = "/files";
        public long MaxFileSize { get; set; } = 10 * 1024 * 1024; // 10MB
        public string[] AllowedExtensions { get; set; } = new[] { ".jpg", ".jpeg", ".png", ".gif", ".pdf", ".doc", ".docx" };
    }
}
