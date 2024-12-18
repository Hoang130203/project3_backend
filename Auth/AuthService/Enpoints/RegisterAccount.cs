using AuthService.Application.CQRS.Commands.Users.CreateUser;
using AuthService.Application.DTOs.Request;

namespace AuthService.Enpoints
{
    public record RegisterAccountRequest (RegisterRequest RegisterRequest);

    public record RegisterAccountResponse(Guid UserId);

    public class RegisterAccount : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/users/register", async (RegisterAccountRequest request, ISender sender) =>
            {
                var command = request.Adapt<CreateUserCommand>();
                var result = await sender.Send(command);

                var response = new RegisterAccountResponse(result.UserId);
                return Results.Created($"/users/{result.UserId}", response);
            })
            .WithName("Register a new user")   
            .Produces<RegisterAccountResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);
        }
    }
}
