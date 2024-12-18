using BuildingBlocks.Messaging.Events;
using NotificationService.Models;

namespace NotificationService.Interfaces
{
    public class WebSocketChannel : INotificationChannel
    {
        private readonly IConnectionManager _connectionManager;

        public string ChannelType => "WEBSOCKET";
        public NotificationPriority Priority => NotificationPriority.Low;


        public async Task DeliverAsync(Notification notification)
        {
            Guid targetUserId = notification.TargetUserId;
            //var message = new NotificationMessage
            //{
            //    NotificationId = notification.Id,
            //    Type = notification.Type,
            //    Data = notification.Data,
            //    CreatedAt = notification.CreatedAt
            //};

        }
    }
}
