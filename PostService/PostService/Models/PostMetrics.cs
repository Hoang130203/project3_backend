using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;
using SocialAppObjects.Enums;
using System.Text.Json.Serialization;

namespace PostService.Models
{
    public class PostMetrics
    {
        [BsonElement("ViewCount")]
        [JsonPropertyName("viewCount")]
        public int ViewCount { get; set; }

        [BsonElement("UniqueViewerCount")]
        [JsonPropertyName("uniqueViewerCount")]
        public int UniqueViewerCount { get; set; }


        [BsonElement("ShareCount")]
        [JsonPropertyName("shareCount")]
        public int ShareCount { get; set; }

        [BsonElement("ReactionCounts")]
        [JsonPropertyName("reactionCounts")]
        [BsonDictionaryOptions(Representation = DictionaryRepresentation.ArrayOfArrays)]
        public Dictionary<ReactionType, int> ReactionCounts { get; set; } = new();

        [BsonElement("CommentCount")]
        [JsonPropertyName("commentCount")]
        public int CommentCount { get; set; }

        [BsonElement("EngagementRate")]
        [JsonPropertyName("engagementRate")]
        public float EngagementRate { get; set; }

        [BsonElement("LastEngagementAt")]
        [JsonPropertyName("lastEngagementAt")]
        public DateTime LastEngagementAt { get; set; }
    }
}
