using BuildingBlocks.Messaging.Events;
using MassTransit;
using ProfileService.Data;
using ProfileService.Models;

namespace ProfileService.EventHandlers.Integration
{
    public class CreateUserEventHandler
        (ILogger<CreateUserEvent> logger,
        IProfileRepository profileRepository
        )
    : IConsumer<CreateUserEvent>
    {
        public async Task Consume(ConsumeContext<CreateUserEvent> context)
        {
            logger.LogInformation(
                   "Profile Service received user created event. UserId: {UserId}, Email: {Email}, Name: {Name}",
                   context.Message.UserId,
                   context.Message.Email,
                   context.Message.FullName
               );
            await profileRepository.StoreProfile(new UserProfile
            {
                Id = context.Message.UserId,
                FullName = context.Message.FullName,
                AvatarUrl = context.Message.ProfilePictureUrl,
                ProfileBackgroundUrl = context.Message.ProfileBackgroundUrl,
                IsMale = context.Message.IsMale,
                Bio = context.Message.Bio,
                Location = context.Message.Location,
                Website = context.Message.Website,
                Email = context.Message.Email,
                Phone = context.Message.Phone,
                maritalStatus = AuthService.Domain.Enums.MaritalStatus.Single,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
            await Task.CompletedTask;
        }
    }
}
