using Grpc.Core;

namespace FileService.Service
{
    public class FileServiceGrpc : FileService.FileServiceBase
    {
        private readonly IFileService _fileService;
        private readonly ILogger<FileServiceImpl> _logger;

        public FileServiceGrpc(IFileService fileService, ILogger<FileServiceImpl> logger)
        {
            _fileService = fileService;
            _logger = logger;
        }
        public override async Task<UploadFileResponse> UploadFile(IAsyncStreamReader<UploadFileRequest> requestStream, ServerCallContext context)
        {
            try
            {
                FileMetadata metadata = null;
                var fileStream = new MemoryStream();

                await foreach (var request in requestStream.ReadAllAsync())
                {
                    if (request.DataCase == UploadFileRequest.DataOneofCase.Metadata)
                    {
                        metadata = request.Metadata;
                    }
                    else if (request.DataCase == UploadFileRequest.DataOneofCase.Chunk)
                    {
                        await fileStream.WriteAsync(request.Chunk.ToByteArray());
                    }
                }

                if (metadata == null)
                {
                    return new UploadFileResponse
                    {
                        Success = false,
                        Error = "No metadata received"
                    };
                }

                var result = await _fileService.UploadAsync(new FormFile(
                    fileStream,
                    0,
                    fileStream.Length,
                    metadata.Filename,
                    metadata.Filename)
                {
                    ContentType = metadata.ContentType
                });

                return new UploadFileResponse
                {
                    StorageType = result.StorageType == Models.StorageType.Local ? true : false,
                    FileUrl = result.Url,
                    Success = result.Success,
                    Error = result.ErrorMessage
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file");
                return new UploadFileResponse
                {
                    Success = false,
                    Error = "Internal server error"
                };
            }
        }
    }
}
