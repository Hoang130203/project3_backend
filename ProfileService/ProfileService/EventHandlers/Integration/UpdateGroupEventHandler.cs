using BuildingBlocks.Messaging.Events;
using MassTransit;
using ProfileService.Data;
using ProfileService.Models;

namespace ProfileService.EventHandlers.Integration
{
    public class UpdateGroupEventHandler
        (ILogger<UpdateGroupEvent> logger,
        IProfileRepository profileRepository
        )
    : IConsumer<UpdateGroupEvent>
    {
        public async Task Consume(ConsumeContext<UpdateGroupEvent> context)
        {
            logger.LogInformation(
                "Profile Service received group update event. GroupId: {GroupId}, Name: {Name}",
                context.Message.GroupId,
                context.Message.Name
            );

            try
            {
                var existingGroup = await profileRepository.GetGroupInfoByIdAsync(
                    context.Message.GroupId,
                    context.CancellationToken
                );

                // Update group info with new values
                existingGroup.Name = context.Message.Name;
                existingGroup.Description = context.Message.Description;
                existingGroup.Visibility = context.Message.Visibility;
                existingGroup.UpdatedAt = context.Message.UpdatedAt;

                // Store updated group info
                await profileRepository.StoreGroupInfo(
                    existingGroup,
                    context.CancellationToken
                );

                logger.LogInformation(
                    "Successfully updated group info in profile service. GroupId: {GroupId}",
                    context.Message.GroupId
                );
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Failed to update group info in profile service. GroupId: {GroupId}",
                    context.Message.GroupId
                );
                throw; // Re-throw to let MassTransit handle the failure
            }
        }
    }
}