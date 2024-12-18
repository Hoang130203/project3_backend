using AuthService.Domain.Entities.Users;
using AuthService.Domain.Enums;

namespace AuthService.Application.Interfaces.Services
{
    public interface IJwtService
    {
        string GenerateToken(User user, UserType userType);
        bool ValidateToken(string token);
    }

}
