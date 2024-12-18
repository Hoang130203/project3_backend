namespace NotificationService.Interfaces
{
    public interface IConnectionManager
    {
        Task SendToUserAsync(string userId, object message);
        Task<IEnumerable<string>> GetUserConnectionsAsync(string userId);
        Task AddConnectionAsync(string userId, string connectionId);
        Task RemoveConnectionAsync(string connectionId);
    }
}
