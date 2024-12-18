using PostService.Models;

namespace PostService.Interfaces
{
    public interface IMediaService
    {
        Task<MediaProcessingResult> ProcessMediaAsync(MediaItem mediaItem);
        Task<MediaItem> UploadMediaAsync(IFormFile file);
        Task DeleteMediaAsync(string mediaId);
    }
}
