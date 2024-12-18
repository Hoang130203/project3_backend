using BuildingBlocks.Messaging.Events;
using MassTransit;
using NotificationService.Interfaces;
using NotificationService.Models;

namespace NotificationService.Services
{
    public class NotificationProcessor : INotificationProcessor , IConsumer<NotificationEvent>
    {
        private readonly INotificationRepository _repository;
        private readonly IEnumerable<INotificationChannel> _channels;
        private readonly ILogger<NotificationProcessor> _logger;

        public async Task Consume(ConsumeContext<NotificationEvent> context)
        {
            await ProcessAsync(context.Message);
        }

        public async Task ProcessAsync(NotificationEvent @event)
        {
            // Get user settings

            // Create notification
            var notification = new Notification
            {
                Type = @event.Type,
                TargetUserId = @event.TargetUserId,
                ActorUserId = @event.ActorUserId,
                Data = @event.Data,
                CreatedAt = DateTime.UtcNow,
                Status = NotificationStatus.Pending
            };

            /// Get user settings and filter channels
            var settings = await _repository.GetUserSettingsAsync(@event.TargetUserId);
            notification.DeliveryChannels = await FilterChannels(notification, settings);

            // Save notification
            await _repository.SaveAsync(notification);

            // Deliver to each channel
            foreach (var channel in _channels.Where(c =>
                notification.DeliveryChannels.Contains(c.ChannelType)))
            {
                try
                {
                    await channel.DeliverAsync(notification);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to deliver notification through {Channel}",
                        channel.ChannelType);
                }
            }
        }

        private async Task<List<string>> FilterChannels(
            Notification notification,
            UserNotificationSettings settings)
        {
            var channels = new List<string>();

            // Check if notification type is enabled
            if (!settings.NotificationTypes.GetValueOrDefault(notification.Type, true))
                return channels;

            // Check quiet hours
            if (settings.DoNotDisturb || IsInQuietHours(settings))
                return channels;

            // Check each channel's settings
            foreach (var channel in _channels)
            {
                var channelSettings = settings.ChannelPreferences
                    .GetValueOrDefault(channel.ChannelType);

                if (channelSettings?.Enabled == true &&
                    channel.Priority >= channelSettings.MinimumPriority)
                {
                    channels.Add(channel.ChannelType);
                }
            }

            return channels;
        }

        private bool IsInQuietHours(UserNotificationSettings settings)
        {
            throw new NotImplementedException();
        }
    }
}
