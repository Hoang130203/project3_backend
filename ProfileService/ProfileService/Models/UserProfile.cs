using AuthService.Domain.Enums;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace ProfileService.Models
{
    public class UserProfile
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public string ProfileBackgroundUrl { get; set; } = string.Empty;
        public string? Bio { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsMale { get; set; } = true;
        public string Location { get; set; } = string.Empty;
        public string Website { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public MaritalStatus maritalStatus { get; set; }
    }
}
