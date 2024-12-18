
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using SocialAppObjects.Enums;

namespace PostService.Models
{
    public class Post
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public List<MediaItem> MediaItems { get; set; } = new();
        public User Author { get; set; }
        public Group? Group { get; set; }
        public List<string> HashTags { get; set; } = new();
        public List<UserTag>? UserTags { get; set; } = new();
        public Location? Location { get; set; }
        public List<Comment> Comments { get; set; } = new();
        public List<Reaction> Reactions { get; set; } = new();
        public List<Share> Shares { get; set; } = new();
        public VisibilitySettings Visibility { get; set; }
        public PostStatus Status { get; set; } = PostStatus.Active;
        public List<Report> Reports { get; set; } = new();
        public PostMetrics Metrics { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
