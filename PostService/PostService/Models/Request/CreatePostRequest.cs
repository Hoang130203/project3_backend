using SocialAppObjects.Enums;

namespace PostService.Models.Request
{
    public class CreatePostRequest
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public List<MediaItemRequest> MediaItems { get; set; } = new();
        public Guid? GroupId { get; set; }
        public VisibilitySettings? VisibilitySettings { get; set; }
    }
    public class MediaItemRequest
    {
        public string ContentType { get; set; }
        public string Url { get; set; }
        public long Size { get; set; }
        public MediaType Type { get; set; }
    }
}
