namespace JwtConfiguration;

public interface IJwtBuilder
{
    string GetToken(Guid userId);
    string GetToken(Guid userId, string userType);
    string ValidateToken(string token);
}