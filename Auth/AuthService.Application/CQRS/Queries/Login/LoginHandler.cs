
using JwtConfiguration;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Application.CQRS.Queries.Login
{
    public class LoginHandler(IApplicationDbContext dbContext, IEncryptor encryptor, IJwtBuilder jwtBuilder)
        : IQueryHandler<LoginQuery, LoginResult>
    {
        public async Task<LoginResult?> Handle(LoginQuery request, CancellationToken cancellationToken)
        {
            var user = await dbContext.Users
                .Include(x => x.Profile)
                .FirstOrDefaultAsync(x => x.Username == request.LoginRequest.Username, cancellationToken);

            if (user is null || !user.ValidatePassword(request.LoginRequest.Password, encryptor))
            {
                return null;
            }

            var token = jwtBuilder.GetToken(user.Id, user.UserType.ToString());
            //var token = "token";

            return new LoginResult(token, user.Profile);
        }
    }
}
