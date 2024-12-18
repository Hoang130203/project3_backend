using MessageService.Models;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Driver;

namespace MessageService.Hubs
{
    public class ChatHub : Hub
    {
        private readonly MongoDbContext _context;
        private static readonly Dictionary<string, string> _connections = new();

        public ChatHub(MongoDbContext context)
        {
            _context = context;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.GetHttpContext()?.Request.Headers["X-UserId"].FirstOrDefault();
            if (!string.IsNullOrEmpty(userId))
            {
                _connections[userId] = Context.ConnectionId;
                await Groups.AddToGroupAsync(Context.ConnectionId, userId);
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.GetHttpContext()?.Request.Headers["X-UserId"].FirstOrDefault();
            if (!string.IsNullOrEmpty(userId))
            {
                _connections.Remove(userId);
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId);
            }
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(Guid conversationId, string content)
        {
            var userId = Context.GetHttpContext()?.Request.Headers["X-UserId"].FirstOrDefault();
            if (string.IsNullOrEmpty(userId))
                throw new HubException("User ID not found");

            var message = new Message
            {
                Id = Guid.NewGuid(),
                ConversationId = conversationId,
                SenderId = Guid.Parse(userId),
                Content = content,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Messages.InsertOneAsync(message);

            // Update conversation last message time
            var update = Builders<Conversation>.Update
                .Set(c => c.LastMessageAt, DateTime.UtcNow);
            await _context.Conversations.UpdateOneAsync(
                c => c.Id == conversationId,
                update
            );

            // Get conversation and notify participants
            var conversation = await _context.Conversations
                .Find(c => c.Id == conversationId)
                .FirstOrDefaultAsync();

            if (conversation != null)
            {
                foreach (var participantId in conversation.ParticipantIds)
                {
                    await Clients.Group(participantId.ToString())
                        .SendAsync("ReceiveMessage", message);
                }
            }
        }
    }

}
