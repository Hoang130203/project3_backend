using FileService.Models;
using Microsoft.Extensions.Options;

namespace FileService.Service
{
    public class HybridFileService : IFileService
    {
        private readonly ILocalFileService _localFileService;
        private readonly IGoogleDriveService _googleDriveService;
        private readonly ILogger<HybridFileService> _logger;
        private const int GOOGLE_DRIVE_THRESHOLD = 50 * 1024 * 1024; // 50MB
        private readonly FileServiceOptions _options;

        public HybridFileService(
            ILocalFileService localFileService,
            IGoogleDriveService googleDriveService,
            IOptions<FileServiceOptions> options,
            ILogger<HybridFileService> logger)
        {
            _localFileService = localFileService;
            _googleDriveService = googleDriveService;
            _logger = logger;
            _options = options.Value;
        }

        public async Task<FileUploadResult> UploadAsync(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return new FileUploadResult { Success = false, ErrorMessage = "No file provided" };

                // Chọn service dựa trên kích thước file
                if (file.Length > GOOGLE_DRIVE_THRESHOLD)
                {
                    _logger.LogInformation($"File size {file.Length} bytes exceeds threshold. Using Google Drive storage.");
                    var driveResult = await _googleDriveService.UploadAsync(file);
                    driveResult.StorageType = StorageType.GoogleDrive;
                    return driveResult;
                }

                _logger.LogInformation($"File size {file.Length} bytes. Using local storage.");
                var localResult = await _localFileService.UploadAsync(file);
                localResult.StorageType = StorageType.Local;
                return localResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in hybrid file upload");
                return new FileUploadResult
                {
                    Success = false,
                    ErrorMessage = "Error uploading file"
                };
            }
        }

        public async Task<(Stream FileStream, string ContentType, string FileName)> GetFileAsync(string fileName)
        {
            var filePath = Path.Combine(_options.StoragePath, fileName);

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"File {fileName} not found");

            var contentType = GetContentType(Path.GetExtension(fileName));
            var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

            return (stream, contentType, fileName);
        }

        private string GetContentType(string extension)
        {
            return extension.ToLower() switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".pdf" => "application/pdf",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                _ => "application/octet-stream"
            };
        }
    }
}
