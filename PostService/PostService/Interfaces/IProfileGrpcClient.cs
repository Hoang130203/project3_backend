using ProfileService.Proto;

namespace PostService.Interfaces
{
    public interface IProfileGrpcClient
    {
        Task<SubInfoResponse> GetSubInfoAsync(string userId);
        Task<MultipleSubInfoResponse> GetMultipleSubInfoAsync(List<string> usersId);
    }
}
