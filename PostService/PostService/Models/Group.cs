using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace PostService.Models
{
    public class Group
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Avatar { get; set; }
    }
}
