using BuildingBlocks.Messaging.Events;
using MassTransit;
using ProfileService.Data;
using ProfileService.Models;

namespace ProfileService.EventHandlers.Integration
{
    public class CreateGroupEventHandler : IConsumer<CreateGroupEvent>
    {
        private readonly ILogger<CreateGroupEventHandler> _logger;
        private readonly IProfileRepository _profileRepository;

        public CreateGroupEventHandler(
            ILogger<CreateGroupEventHandler> logger,
            IProfileRepository profileRepository)
        {
            _logger = logger;
            _profileRepository = profileRepository;
        }

        public async Task Consume(ConsumeContext<CreateGroupEvent> context)
        {
            try
            {
                _logger.LogInformation(
                    "Profile Service received group created event. GroupId: {GroupId}, Name: {Name}, OwnerId: {OwnerId}",
                    context.Message.GroupId,
                    context.Message.Name,
                    context.Message.OwnerId
                );

                var groupInfo = new GroupInfo
                {
                    Id = context.Message.GroupId,
                    Name = context.Message.Name,
                    Description = context.Message.Description,
                    OwnerId = context.Message.OwnerId,
                    OwnerName = context.Message.OwnerName,
                    CreatedAt = context.Message.CreatedAt,
                    UpdatedAt = DateTime.UtcNow,
                    Visibility = context.Message.Visibility,
                    GroupPictureUrl = string.Empty,
                    GroupBackgroundUrl = string.Empty,
                    IsDeleted = false,
                    IsLocked = false
                };

                await _profileRepository.StoreGroupInfo(groupInfo);
                _logger.LogInformation("Successfully stored group info for group {GroupId}", groupInfo.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing CreateGroupEvent for group {GroupId}", context.Message.GroupId);
                throw;
            }
        }
    }
}
