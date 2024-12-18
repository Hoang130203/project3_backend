using MessageService.Interfaces;
using MessageService.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace MessageService.Controllers
{
    [ApiController]
    [Route("api/messages")]
    public class MessagesController : ControllerBase
    {
        private readonly MongoDbContext _context;
        private readonly IProfileGrpcClient profileGrpcClient;
        public MessagesController(MongoDbContext context,
            IProfileGrpcClient profileGrpcClient
            )
        {
            _context = context;
            this.profileGrpcClient = profileGrpcClient;
        }

        [HttpGet("conversations")]
        public async Task<IActionResult> GetConversations()
        {
            var userId = HttpContext.Request.Headers["X-UserId"].FirstOrDefault();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var userGuid = Guid.Parse(userId);
            var conversations = await _context.Conversations
                .Find(c => c.ParticipantIds.Contains(userGuid))
                .SortByDescending(c => c.LastMessageAt)
                .ToListAsync();

            var conversationDtos = new List<ConversationDto>();

            foreach (var c in conversations)
            {
                var participantIds = c.ParticipantIds
                    .Where(p => p != userGuid)
                    .Select(p => p.ToString())
                    .ToList();

                var subInfoResponse = await profileGrpcClient.GetMultipleSubInfoAsync(participantIds);

                conversationDtos.Add(new ConversationDto
                {
                    Id = c.Id,
                    Participants = subInfoResponse.SubInfos.Select(s => new ParticipantDto
                    {
                        Id = s.Id,
                        Username = s.Name,
                        Avatar = s.AvatarUrl
                    }).ToList(),
                    CreatedAt = c.CreatedAt,
                    LastMessageAt = c.LastMessageAt
                });
            }

            return Ok(conversationDtos);
        }


        [HttpGet("conversations/{conversationId}/messages")]
        public async Task<IActionResult> GetMessages(Guid conversationId)
        {
            var userId = HttpContext.Request.Headers["X-UserId"].FirstOrDefault();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var messages = await _context.Messages
                .Find(m => m.ConversationId == conversationId)
                .SortBy(m => m.CreatedAt)
                .ToListAsync();
            if(messages == null)
                return Ok(new object[0]);
            return Ok(messages);
        }

        [HttpPost("conversations")]
        public async Task<IActionResult> CreateConversation([FromBody] List<Guid> participantIds)
        {
            var userId = HttpContext.Request.Headers["X-UserId"].FirstOrDefault();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            // Include the creator in participants if not already included
            if (!participantIds.Contains(Guid.Parse(userId)))
            {
                participantIds.Add(Guid.Parse(userId));
            }

            var conversation = new Conversation
            {
                Id = Guid.NewGuid(),
                ParticipantIds = participantIds,
                CreatedAt = DateTime.UtcNow,
                LastMessageAt = DateTime.UtcNow
            };

            await _context.Conversations.InsertOneAsync(conversation);
            return Ok(conversation);
        }
    }
    public class ConversationDto
    {
        public Guid Id { get; set; }
        public List<ParticipantDto> Participants { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastMessageAt { get; set; }
    }
    public class ParticipantDto
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Avatar { get; set; }
    }
}
