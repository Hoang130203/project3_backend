using BuildingBlocks.Messaging.Events;
using NotificationService.Models;

namespace NotificationService.Interfaces
{
    public interface INotificationChannel
    {
        string ChannelType { get; }
        NotificationPriority Priority { get; }
        Task DeliverAsync(Notification notification);
    }
}
