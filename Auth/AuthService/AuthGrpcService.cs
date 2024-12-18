using Auth;
using AuthService.Application.Interfaces.Repositories;
using AuthService.Application.Interfaces.Services;
using AuthService.Domain.Enums;
using Grpc.Core;
using JwtConfiguration;

namespace AuthService
{
    public class AuthGrpcService : AuthServiceGrpc.AuthServiceGrpcBase
    {
        private readonly IAuthorizationService _authService;
        private readonly IJwtBuilder _jwtBuilder;
        private readonly ILogger<AuthGrpcService> _logger;
        private readonly IUserRepository _userRepository;
        public AuthGrpcService(
            IAuthorizationService authService,
            IJwtBuilder jwtBuilder,
            ILogger<AuthGrpcService> logger,
            IUserRepository userRepository)
        {
            _authService = authService;
            _jwtBuilder = jwtBuilder;
            _logger = logger;
            _userRepository = userRepository;
        }
        public override async Task<ValidateTokenResponse> ValidateToken(
       ValidateTokenRequest request,
       ServerCallContext context)
        {
            try
            {
                var userId = _jwtBuilder.ValidateToken(request.Token);
                if (string.IsNullOrEmpty(userId))
                {
                    return new ValidateTokenResponse { IsValid = false };
                }

                // Get user type for additional validation
                var user = await _userRepository.GetUserByIdAsync(Guid.Parse(userId));

                return new ValidateTokenResponse
                {
                    IsValid = true,
                    UserId = userId,
                    UserType = user.UserType.ToString()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating token");
                return new ValidateTokenResponse { IsValid = false };
            }
        }

        public override async Task<PermissionResponse> CheckSystemPermission(
            SystemPermissionRequest request,
            ServerCallContext context)
        {
            try
            {
                var permissionType = Enum.Parse<PermissionType>(request.Permission);
                var isAllowed = await _authService.AuthorizeSystemAccessAsync(
                    Guid.Parse(request.UserId),
                    permissionType);

                return new PermissionResponse { IsAllowed = isAllowed };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking system permission");
                return new PermissionResponse { IsAllowed = false };
            }
        }

        public override async Task<PermissionResponse> CheckGroupPermission(
            GroupPermissionRequest request,
            ServerCallContext context)
        {
            try
            {
                var permissionType = Enum.Parse<PermissionType>(request.Permission);
                var isAllowed = await _authService.AuthorizeGroupAccessAsync(
                    Guid.Parse(request.UserId),
                    Guid.Parse(request.GroupId),
                    permissionType);

                return new PermissionResponse { IsAllowed = isAllowed };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking group permission");
                return new PermissionResponse { IsAllowed = false };
            }
        }
    }
}
