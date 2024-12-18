using Microsoft.AspNetCore.SignalR;
using NotificationService.Hubs;
using NotificationService.Interfaces;
using System.Collections.Concurrent;

namespace NotificationService.Services
{
    public class ConnectionManager : IConnectionManager
    {
        private readonly ConcurrentDictionary<string, HashSet<string>> _userConnections = new();
        private readonly ConcurrentDictionary<string, string> _connectionMap = new();
        private readonly IHubContext<NotificationHub> _hubContext;

        public ConnectionManager(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendToUserAsync(string userId, object message)
        {
            if (_userConnections.TryGetValue(userId, out var connections))
            {
                await _hubContext.Clients.Clients(connections.ToList())
                    .SendAsync("ReceiveNotification", message);
            }
        }

        public async Task<IEnumerable<string>> GetUserConnectionsAsync(string userId)
        {
            return _userConnections.TryGetValue(userId, out var connections)
                ? connections
                : Enumerable.Empty<string>();
        }

        public Task AddConnectionAsync(string userId, string connectionId)
        {
            _userConnections.AddOrUpdate(
                userId,
                _ => new HashSet<string> { connectionId },
                (_, connections) =>
                {
                    connections.Add(connectionId);
                    return connections;
                });

            _connectionMap.TryAdd(connectionId, userId);
            return Task.CompletedTask;
        }

        public Task RemoveConnectionAsync(string connectionId)
        {
            if (_connectionMap.TryRemove(connectionId, out var userId))
            {
                if (_userConnections.TryGetValue(userId, out var connections))
                {
                    connections.Remove(connectionId);
                    if (connections.Count == 0)
                    {
                        _userConnections.TryRemove(userId, out _);
                    }
                }
            }
            return Task.CompletedTask;
        }
    }
}
