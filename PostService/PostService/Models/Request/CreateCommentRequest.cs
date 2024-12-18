namespace PostService.Models.Request
{
    public class CreateCommentRequest
    {
        public string Content { get; set; }
        public List<MediaItemRequest> MediaItems { get; set; } = new();
        public List<Guid> UserTags { get; set; } = new();

        public string? ParentComment { get; set; }
    }
}
