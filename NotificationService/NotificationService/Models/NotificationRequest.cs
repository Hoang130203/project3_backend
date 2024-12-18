using BuildingBlocks.Messaging.Events;

namespace NotificationService.Models
{
    public class NotificationRequest
    {
        public string Type { get; set; }  // Like, Comment, Follow, etc.
        public string TargetUserId { get; set; }
        public string ActorUserId { get; set; }
        public Dictionary<string, object> Data { get; set; }
        public NotificationPriority Priority { get; set; }
        public List<string> Channels { get; set; } // WebSocket, Push, Email, SMS
    }

}
