using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace PostService.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid UserId { get; set; }
        public string? Name { get; set; }
        public string? Avatar { get; set; }
    }

}
