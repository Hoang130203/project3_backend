


using Grpc.Core;
using Grpc.Net.Client;
using PostService.Interfaces;
using PostService.Protos;

namespace PostService.Services
{
    public class ConnectionGrpcClientService : IConnectionGrpcClient
    {
        private readonly ConnectionServiceGrpc.ConnectionServiceGrpcClient _client;
        private readonly ILogger<ConnectionGrpcClientService> _logger;

        public ConnectionGrpcClientService(
            ILogger<ConnectionGrpcClientService> logger)
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

            _client = new ConnectionServiceGrpc.ConnectionServiceGrpcClient(channel);
        }

        public async Task<UserConnectionsResponse> GetUserConnectionsAsync(string userId)
        {
            try
            {
                var request = new UserConnectionsRequest { UserId = userId };
                _logger.LogInformation("Sending gRPC request to get user connections for userId: {UserId}", userId);

                var response = await _client.GetUserConnectionsAsync(request);
                _logger.LogInformation("Received response with {ConnectionCount} connections", response.ToString());

                return response;
            }
            catch (RpcException ex)
            {
                _logger.LogError(ex, "gRPC call failed with status: {StatusCode} and message: {Message}", ex.StatusCode, ex.Message);
                throw; // Hoặc trả về một lỗi phù hợp
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while calling GetUserConnectionsAsync");
                throw;
            }
        }


    }
}
