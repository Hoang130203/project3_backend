using FileService.Models;

namespace FileService.Service
{
    public interface IFileService
    {
        Task<FileUploadResult> UploadAsync(IFormFile file);
        Task<(Stream FileStream, string ContentType, string FileName)> GetFileAsync(string fileName);
    }
}
