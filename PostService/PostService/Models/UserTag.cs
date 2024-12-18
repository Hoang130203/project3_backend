using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace PostService.Models
{
    public class UserTag
    {
        [BsonRepresentation(BsonType.String)]
        public Guid UserId { get; set; }
        public string? UserName { get; set; }
        public float? XPosition { get; set; }  // For tagging in photos
        public float? YPosition { get; set; }
    }
}
