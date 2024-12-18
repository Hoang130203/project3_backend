using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using SocialAppObjects.Enums;

namespace PostService.Models
{
    public class VisibilitySettings
    {
        public VisibilityType Type { get; set; }
        [BsonRepresentation(BsonType.String)]
        public List<Guid> AllowedUserIds { get; set; } = new();

        [BsonRepresentation(BsonType.String)]
        public List<Guid> ExcludedUserIds { get; set; } = new();

        public bool AllowComments { get; set; } = true;
        public bool AllowReactions { get; set; } = true;
        public bool AllowSharing { get; set; } = true;
    }

}
