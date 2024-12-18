using AuthService.Domain.Entities.Users;
using AuthService.Domain.Enums;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace ProfileService.Models
{
    public class GroupInfo
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        [BsonRepresentation(BsonType.String)]
        public Guid OwnerId { get; set; }
        public string OwnerName { get; set; } = string.Empty;
        public string GroupPictureUrl { get; set; } = string.Empty;
        public string GroupBackgroundUrl { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public GroupVisibility Visibility { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsLocked { get; set; }

    }
}
