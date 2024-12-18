using SocialAppObjects.Enums;

namespace PostService.Models
{
    public class Comment
    {
        public string Id { get; set; }
        public string Content { get; set; }
        public User Author { get; set; }
        public List<MediaItem> MediaItems { get; set; } = new();
        public List<UserTag> UserTags { get; set; } = new();
        public List<Reaction> Reactions { get; set; } = new();
        public string? ParentComment { get; set; }  // For nested comments
        public CommentStatus Status { get; set; } = CommentStatus.Active;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

}
