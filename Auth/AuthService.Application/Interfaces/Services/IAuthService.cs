using AuthService.Application.DTOs.Request;
using AuthService.Domain.Entities.Users;
using AuthService.Domain.Enums;


namespace AuthService.Application.Interfaces.Services
{
    public interface IAuthenticationService
    {
        Task<string> RegisterUserAsync(RegisterRequest registerRequest);
        Task<string> LoginAsync(LoginRequest loginRequest);
    }
    public interface IAuthorizationService
    {
        Task<bool> AuthorizeSystemAccessAsync(Guid userId, PermissionType permission);
        Task<bool> AuthorizeGroupAccessAsync(Guid userId, Guid groupId, PermissionType permission);
        Task<bool> AuthorizeMultiplePermissionsAsync(
        Guid userId,
        Guid? groupId,
        IEnumerable<PermissionType> permissions);
    }
}
