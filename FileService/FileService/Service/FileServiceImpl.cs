using FileService.Models;
using Microsoft.Extensions.Options;

namespace FileService.Service
{
    public interface ILocalFileService
    {
        Task<FileUploadResult> UploadAsync(IFormFile file);
    }
    public class FileServiceImpl : ILocalFileService
    {
        private readonly FileServiceOptions _options;
        private readonly ILogger<FileServiceImpl> _logger;

        public FileServiceImpl(IOptions<FileServiceOptions> options, ILogger<FileServiceImpl> logger)
        {
            _options = options.Value;
            _logger = logger;

            // Đảm bảo thư mục uploads tồn tại
            Directory.CreateDirectory(_options.StoragePath);
        }

        public async Task<FileUploadResult> UploadAsync(IFormFile file)
        {
            try
            {
                // Validate file
                if (file == null || file.Length == 0)
                    return new FileUploadResult { Success = false, ErrorMessage = "No file provided" };

                if (file.Length > _options.MaxFileSize)
                    return new FileUploadResult { Success = false, ErrorMessage = "File size exceeds limit" };

                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!_options.AllowedExtensions.Contains(extension))
                    return new FileUploadResult { Success = false, ErrorMessage = "File type not allowed" };

                // Tạo tên file an toàn, tránh trùng lặp
                var uniqueFileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(_options.StoragePath, uniqueFileName);

                // Lưu file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Trả về kết quả
                return new FileUploadResult
                {
                    Success = true,
                    FilePath = filePath,
                    Url = $"{_options.BaseUrl}/{uniqueFileName}",
                    StorageType = StorageType.Local
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file {FileName}", file.FileName);
                return new FileUploadResult
                {
                    Success = false,
                    ErrorMessage = "Error uploading file"
                };
            }
        }


    }
}
