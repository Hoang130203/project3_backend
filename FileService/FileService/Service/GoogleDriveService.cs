using FileService.Models;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Microsoft.Extensions.Options;

namespace FileService.Service
{
    public class GoogleDriveOptions
    {
        public string CredentialsPath { get; set; } = string.Empty;
        public string FolderId { get; set; } = string.Empty;
    }

    public interface IGoogleDriveService
    {
        Task<FileUploadResult> UploadAsync(IFormFile file);
    }

    public class GoogleDriveService : IGoogleDriveService
    {
        private readonly DriveService _driveService;
        private readonly string _folderId;
        private readonly ILogger<GoogleDriveService> _logger;

        public GoogleDriveService(
            IOptions<GoogleDriveOptions> options,
            ILogger<GoogleDriveService> logger)
        {
            _logger = logger;
            _folderId = options.Value.FolderId;

            try
            {
                var credential = GoogleCredential
                    .FromFile(options.Value.CredentialsPath)
                    .CreateScoped(DriveService.ScopeConstants.DriveFile);

                _driveService = new DriveService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = credential
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing Google Drive service");
                throw;
            }
        }

        public async Task<FileUploadResult> UploadAsync(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return new FileUploadResult
                    {
                        Success = false,
                        ErrorMessage = "No file provided"
                    };
                }

                var fileMetadata = new Google.Apis.Drive.v3.Data.File()
                {
                    Name = $"{Guid.NewGuid()}_{file.FileName}",
                    Parents = new List<string> { _folderId }
                };

                using var stream = file.OpenReadStream();
                var request = _driveService.Files.Create(fileMetadata, stream, file.ContentType);
                request.Fields = "id, webViewLink";

                var upload = await request.UploadAsync();
                if (upload.Status != Google.Apis.Upload.UploadStatus.Completed)
                {
                    return new FileUploadResult
                    {
                        Success = false,
                        ErrorMessage = "Upload failed"
                    };
                }

                // Set file to be publicly accessible
                var permission = new Google.Apis.Drive.v3.Data.Permission
                {
                    Type = "anyone",
                    Role = "reader"
                };
                await _driveService.Permissions.Create(permission, request.ResponseBody.Id).ExecuteAsync();

                return new FileUploadResult
                {
                    Success = true,
                    Url = request.ResponseBody.WebViewLink
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file to Google Drive");
                return new FileUploadResult
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }
    }
}
