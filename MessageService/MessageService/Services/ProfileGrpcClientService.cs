using Grpc.Net.Client;
using MessageService.Interfaces;

using ProfileService.Proto;

namespace PostService.Services
{
    public class ProfileGrpcClientService : IProfileGrpcClient
    {
        private readonly ProfileServiceGrpc.ProfileServiceGrpcClient _client;
        private readonly ILogger<ProfileGrpcClientService> _logger;

        public ProfileGrpcClientService(
            ILogger<ProfileGrpcClientService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            // Cấu hình gRPC channel
            var channel = GrpcChannel.ForAddress("http://profileservice:5001", new GrpcChannelOptions
            {
                HttpHandler = new SocketsHttpHandler
                {
                    EnableMultipleHttp2Connections = true // Hỗ trợ HTTP/2
                }
            });

            _client = new ProfileServiceGrpc.ProfileServiceGrpcClient(channel);
        }

        public async Task<SubInfoResponse> GetSubInfoAsync(string userId)
        {
            try
            {
                _logger.LogInformation("Requesting sub info for user {UserId}", userId);

                var request = new SubInfoRequest { Id = userId };
                var response = await _client.GetSubInfoAsync(request);

                _logger.LogInformation("Successfully retrieved sub info for user {UserId}", userId);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting sub info for user {UserId}", userId);
                throw;
            }
        }

        public async Task<MultipleSubInfoResponse> GetMultipleSubInfoAsync(List<string> userIds)
        {
            try
            {
                _logger.LogInformation("Requesting sub info for {Count} users", userIds.Count);

                var request = new MultipleSubInfoRequest();
                request.Ids.AddRange(userIds);

                var response = await _client.GetMultipleSubInfoAsync(request);

                _logger.LogInformation("Successfully retrieved sub info for {Count} users", response.SubInfos.Count);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting multiple sub info for {Count} users", userIds.Count);
                throw;
            }
        }


    }
}
