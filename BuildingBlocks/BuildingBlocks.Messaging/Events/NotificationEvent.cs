using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildingBlocks.Messaging.Events
{
    public class NotificationEvent
    {
        public string Type { get; set; }  // COMMENT, LIKE, FOLLOW, MESSAGE
        public Guid TargetUserId { get; set; }
        public Guid ActorUserId { get; set; }
        public Dictionary<string, object> Data { get; set; }
        public NotificationPriority Priority { get; set; }
        public List<string> Channels { get; set; }
    }

    public enum NotificationPriority
    {
        Low,
        Medium,
        High,
        Critical
    }
    public enum NotificationStatus
    {
        Pending,
        Delivered,
        Failed,
        Read
    }
}
