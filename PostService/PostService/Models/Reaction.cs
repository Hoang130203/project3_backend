using SocialAppObjects.Enums;

namespace PostService.Models
{
    public class Reaction
    {
        public string ReactionId { get; set; }
        public string UserId { get; set; }
        public ReactionType Type { get; set; }  
        public DateTime CreatedAt { get; set; }
    }
}
