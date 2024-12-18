using ProfileService.Proto;

namespace MessageService.Interfaces
{
    public interface IProfileGrpcClient
    {
        Task<SubInfoResponse> GetSubInfoAsync(string userId);
        Task<MultipleSubInfoResponse> GetMultipleSubInfoAsync(List<string> usersId);
    }
}
