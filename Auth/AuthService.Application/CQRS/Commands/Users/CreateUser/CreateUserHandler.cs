using JwtConfiguration;
using Mapster;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
namespace AuthService.Application.CQRS.Commands.Users.CreateUser;

public class CreateUserHandler(IApplicationDbContext dbContext,ILogger<CreateUserHandler> logger, IEncryptor encryptor, IPublishEndpoint pubblishEndpoint)
    : ICommandHandler<CreateUserCommand, CreateUserResult>
{
    public async Task<CreateUserResult> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var user = new User
        {
            Email = request.RegisterRequest.Email,
            Username = request.RegisterRequest.Username,
            UserType = request.RegisterRequest.UserType,
            Profile = new UserProfile
            {
                FullName = request.RegisterRequest.FullName,
                ProfilePictureUrl = request.RegisterRequest.AvatarUrl ?? "https://www.gravatar.com/avatar/",
                Email = request.RegisterRequest.Email
            }
        };
        user.SetPassword(request.RegisterRequest.Password, encryptor);
        try
        {
            var existingUser = await dbContext.Users.FirstOrDefaultAsync(x => x.Username == user.Username, cancellationToken);
            if (existingUser != null)
                throw new InvalidOperationException("Email already exists");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error checking if email exists");
            throw;
        }
        await dbContext.Users.AddAsync(user, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        //send to queue
        var eventMessage = user.Profile.Adapt<CreateUserEvent>();
        logger.LogInformation("Publishing user created event for {Email}", user.Email);
        await pubblishEndpoint.Publish(eventMessage, cancellationToken);
        logger.LogInformation("Published user created event");


        return new CreateUserResult(user.Id);
    }
}