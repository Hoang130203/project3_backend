
using SocialAppObjects.Enums;

namespace PostService.Models
{
    public class MediaItem
    {
        public string Id { get; set; }
        public MediaType Type { get; set; }
        public string Url { get; set; }
        public string ThumbnailUrl { get; set; }
        public string ContentType { get; set; }
        public long Size { get; set; }
    }
}
