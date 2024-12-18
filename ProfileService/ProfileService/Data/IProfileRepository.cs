using ProfileService.Models;
using SocialAppObjects;

namespace ProfileService.Data
{
    public interface IProfileRepository
    {
        Task<UserProfile> GetUserProfileByIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<UserProfile>> GetUserProfilesAsync(CancellationToken cancellationToken = default);
        Task<UserProfile> StoreProfile(UserProfile userProfile, CancellationToken cancellationToken = default);
        Task<bool> DeleteProfile(Guid userId, CancellationToken cancellationToken = default);


        //Group
        Task<GroupInfo> GetGroupInfoByIdAsync(Guid groupId, CancellationToken cancellationToken = default);
        Task<IEnumerable<GroupInfo>> GetGroupInfosAsync(CancellationToken cancellationToken = default);
        Task<GroupInfo> StoreGroupInfo(GroupInfo groupInfo, CancellationToken cancellationToken = default);
        Task<bool> DeleteGroupInfo(Guid groupId, CancellationToken cancellationToken = default);


        //get name & avatar

        Task<SubInfo> GetUserNameAndAvatarByIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<SubInfo> GetGroupNameAndAvatarByIdAsync(Guid groupId, CancellationToken cancellationToken = default);
        Task<IEnumerable<SubInfo>> SearchUserByName(string name, CancellationToken cancellationToken = default);

    }
}
