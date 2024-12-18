


using PostService.Protos;

namespace PostService.Interfaces
{
    public interface IConnectionGrpcClient
    {
        Task<UserConnectionsResponse> GetUserConnectionsAsync(string userId);
    }
}
