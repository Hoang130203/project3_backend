using ApiGateway.Proto;
using Grpc.Core;
using Grpc.Net.Client;

namespace ApiGateway.Services
{
    public class AuthGrpcClientService : IAuthGrpcClient
    {
        private readonly AuthServiceGrpc.AuthServiceGrpcClient _client;
        private readonly ILogger<AuthGrpcClientService> _logger;

        public AuthGrpcClientService(
            ILogger<AuthGrpcClientService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            // Cấu hình gRPC channel
            var channel = GrpcChannel.ForAddress("http://authservice:5001", new GrpcChannelOptions
            {
                HttpHandler = new SocketsHttpHandler
                {
                    EnableMultipleHttp2Connections = true // Hỗ trợ HTTP/2
                }
            });

            _client = new AuthServiceGrpc.AuthServiceGrpcClient(channel);
        }

        public async Task<ValidateTokenResponse> ValidateTokenAsync(string token)
        {
            try
            {
                var request = new ValidateTokenRequest { Token = token };
                return await _client.ValidateTokenAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating token through gRPC");
                throw;
            }
        }

        public async Task<PermissionResponse> CheckSystemPermissionAsync(string userId, string permissionType)
        {
            try
            {
                var request = new SystemPermissionRequest
                {
                    UserId = userId,
                    Permission = permissionType
                };
                return await _client.CheckSystemPermissionAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking system permission through gRPC");
                throw;
            }
        }

        public async Task<PermissionResponse> CheckGroupPermissionAsync(string userId, string groupId, string permissionType)
        {
            try
            {
                var request = new GroupPermissionRequest
                {
                    UserId = userId,
                    GroupId = groupId,
                    Permission = permissionType
                };
                return await _client.CheckGroupPermissionAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking group permission through gRPC");
                throw;
            }
        }
    }
}
