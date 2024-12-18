using Microsoft.AspNetCore.SignalR;
using NotificationService.Interfaces;
using System.Security.Claims;

namespace NotificationService.Hubs
{
    public class NotificationHub : Hub
    {
        private readonly IConnectionManager _connectionManager;
        private Logger<NotificationHub> Logger { get; }
        public NotificationHub(IConnectionManager connectionManager, Logger<NotificationHub> logger)
        {
            _connectionManager = connectionManager;
            Logger = logger;
        }
        //public async Task SendNotification(NotificationMessage message)
        //{
        //    await Clients.All.SendAsync("ReceiveNotification", message);
        //}
        public override async Task OnConnectedAsync()
        {
            Console.WriteLine(4336);

            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Logger.LogInformation($"User {userId} connected to the hub");
            if (!string.IsNullOrEmpty(userId))
            {
                await _connectionManager.AddConnectionAsync(userId, Context.ConnectionId);
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await _connectionManager.RemoveConnectionAsync(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }
    }
}
