using MongoDB.Driver;
using NotificationService.Interfaces;
using NotificationService.Models;
using StackExchange.Redis;
using System.Text.Json;

namespace NotificationService.Services
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly IMongoCollection<Notification> _notifications;
        private readonly IMongoCollection<UserNotificationSettings> _settings;
        private readonly IConnectionMultiplexer _redis;

        public Task<IEnumerable<Notification>> GetUserNotificationsAsync(Guid userId, int skip, int take)
        {
            throw new NotImplementedException();
        }

        public Task<UserNotificationSettings> GetUserSettingsAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task MarkAsReadAsync(string notificationId)
        {
            throw new NotImplementedException();
        }

        public async Task SaveAsync(Notification notification)
        {
            await _notifications.InsertOneAsync(notification);

            // Cache recent notifications
            var cache = _redis.GetDatabase();
            var cacheKey = $"notifications:{notification.TargetUserId}";
            await cache.ListLeftPushAsync(cacheKey, JsonSerializer.Serialize(notification));
            await cache.ListTrimAsync(cacheKey, 0, 99); // Keep last 100
        }

        public Task UpdateUserSettingsAsync(UserNotificationSettings settings)
        {
            throw new NotImplementedException();
        }
    }
}
