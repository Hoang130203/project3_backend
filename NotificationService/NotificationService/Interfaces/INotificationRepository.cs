using NotificationService.Models;

namespace NotificationService.Interfaces
{
    public interface INotificationRepository
    {
        Task SaveAsync(Notification notification);
        Task<IEnumerable<Notification>> GetUserNotificationsAsync(Guid userId, int skip, int take);
        Task MarkAsReadAsync(string notificationId);
        Task<UserNotificationSettings> GetUserSettingsAsync(Guid userId);
        Task UpdateUserSettingsAsync(UserNotificationSettings settings);
    }
}
