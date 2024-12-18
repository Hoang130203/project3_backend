using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.CQRS.Queries.Login
{
    public record LoginQuery(LoginRequest LoginRequest)
        : IQuery<LoginResult>;

    public record LoginResult(string Token, UserProfile UserProfile);
}
