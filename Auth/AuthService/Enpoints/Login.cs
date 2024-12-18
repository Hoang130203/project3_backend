using AuthService.Application.CQRS.Queries.Login;
using AuthService.Application.DTOs.Request;
using AuthService.Domain.Entities.Users;

namespace AuthService.Enpoints
{
    public record LoginRequest_(LoginRequest LoginRequest);

    public record LoginResponse(string Token, UserProfile UserProfile);

    public class Login : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/users/login", async (LoginRequest_ request, ISender sender) =>
            {
                var command = request.Adapt<LoginQuery>();
                var result = await sender.Send(command);
                if (result == null)
                {
                    return Results.Unauthorized();
                }
                var response = new LoginResponse(result.Token, result.UserProfile);
                return Results.Ok(response);
            })
            .WithName("Login a user")
            .Produces<LoginResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status500InternalServerError);
        }
    }
}
