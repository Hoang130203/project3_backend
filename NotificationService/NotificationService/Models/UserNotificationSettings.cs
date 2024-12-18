using BuildingBlocks.Messaging.Events;

namespace NotificationService.Models
{
    public class UserNotificationSettings
    {
        public Guid UserId { get; set; }
        public Dictionary<string, ChannelSettings> ChannelPreferences { get; set; }
        public Dictionary<string, bool> NotificationTypes { get; set; }
        public bool DoNotDisturb { get; set; }
        public TimeSpan? QuietHoursStart { get; set; }
        public TimeSpan? QuietHoursEnd { get; set; }
    }
    public class ChannelSettings
    {
        public bool Enabled { get; set; }
        public NotificationPriority MinimumPriority { get; set; }
    }
}
