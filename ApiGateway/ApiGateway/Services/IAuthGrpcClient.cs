using ApiGateway.Proto;


namespace ApiGateway.Services
{
    public interface IAuthGrpcClient
    {
        Task<ValidateTokenResponse> ValidateTokenAsync(string token);
        Task<PermissionResponse> CheckSystemPermissionAsync(string userId, string permissionType);
        Task<PermissionResponse> CheckGroupPermissionAsync(string userId, string groupId, string permissionType);
    }

}
