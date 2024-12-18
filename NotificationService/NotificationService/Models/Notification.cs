using BuildingBlocks.Messaging.Events;

namespace NotificationService.Models
{
    public class Notification
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public Guid TargetUserId { get; set; }
        public Guid ActorUserId { get; set; }
        public Dictionary<string, object> Data { get; set; }
        public NotificationStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? ReadAt { get; set; }
        public List<string> DeliveryChannels { get; set; } = new();

    }

}
