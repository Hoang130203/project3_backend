using BuildingBlocks.Messaging.Events;

namespace NotificationService.Interfaces
{
    public interface INotificationProcessor
    {
        Task ProcessAsync(NotificationEvent notification);
    }
}
