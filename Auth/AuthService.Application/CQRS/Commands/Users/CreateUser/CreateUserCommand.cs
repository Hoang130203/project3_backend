namespace AuthService.Application.CQRS.Commands.Users.CreateUser
{
    public record CreateUserCommand(RegisterRequest RegisterRequest)
       : ICommand<CreateUserResult>;

    public record CreateUserResult(Guid UserId);

    public class CreateUserCommandHandler : AbstractValidator<CreateUserCommand>
    {
        public CreateUserCommandHandler()
        {
            RuleFor(x => x.RegisterRequest).NotNull();
            RuleFor(x => x.RegisterRequest.Email).NotNull().NotEmpty();
            RuleFor(x => x.RegisterRequest.Password).NotNull().NotEmpty();
            RuleFor(x => x.RegisterRequest.Username).NotNull().NotEmpty();
            RuleFor(x => x.RegisterRequest.FullName).NotNull().NotEmpty();
            RuleFor(x => x.RegisterRequest.UserType).NotNull().NotEmpty();
            //RuleFor(x => x.RegisterRequest.AvatarUrl).NotNull().NotEmpty();
        }
    }
}
