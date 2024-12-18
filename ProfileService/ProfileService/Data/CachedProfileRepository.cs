using Microsoft.Extensions.Caching.Distributed;
using ProfileService.Models;
using SocialAppObjects;
using System.Text.Json;

namespace ProfileService.Data
{
    public class CachedProfileRepository
        (IProfileRepository profileRepository, IDistributedCache cache,
        ILogger<CachedProfileRepository> logger
        )
        : IProfileRepository
    {
        private readonly DistributedCacheEntryOptions options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10 * 60) // 5 giờ
        };
        public async Task<bool> DeleteGroupInfo(Guid groupId, CancellationToken cancellationToken = default)
        {
            await profileRepository.DeleteGroupInfo(groupId, cancellationToken);
            await cache.RemoveAsync(groupId.ToString(), cancellationToken);
            return true;
        }

        public async Task<bool> DeleteProfile(Guid userId, CancellationToken cancellationToken = default)
        {
            await profileRepository.DeleteProfile(userId, cancellationToken);
            await cache.RemoveAsync(userId.ToString(), cancellationToken);
            return true;
        }

        public async Task<GroupInfo> GetGroupInfoByIdAsync(Guid groupId, CancellationToken cancellationToken = default)
        {
            var cachedGroupInfo = await cache.GetStringAsync(groupId.ToString(), cancellationToken);
            if (!string.IsNullOrEmpty(cachedGroupInfo))
            {
                logger.LogInformation("GroupInfo with id {GroupId} found in cache", groupId);
                return JsonSerializer.Deserialize<GroupInfo>(cachedGroupInfo)!;
            }
            logger.LogInformation($"{groupId} not found in cache");
            var groupInfo = await profileRepository.GetGroupInfoByIdAsync(groupId, cancellationToken);
            await cache.SetStringAsync(groupId.ToString(), JsonSerializer.Serialize(groupInfo), options, cancellationToken);
            return groupInfo;
        }

        public async Task<IEnumerable<GroupInfo>> GetGroupInfosAsync(CancellationToken cancellationToken = default)
        {
            var cachedGroupInfos = await cache.GetStringAsync("GroupInfos", cancellationToken);
            if (!string.IsNullOrEmpty(cachedGroupInfos))
            {
                logger.LogInformation("GroupInfos found in cache");
                return JsonSerializer.Deserialize<IEnumerable<GroupInfo>>(cachedGroupInfos)!;
            }
            logger.LogInformation("GroupInfos not found in cache");
            var groupInfos = await profileRepository.GetGroupInfosAsync(cancellationToken);
            await cache.SetStringAsync("GroupInfos", JsonSerializer.Serialize(groupInfos), options, cancellationToken);
            return groupInfos;
        }



        public async Task<UserProfile> GetUserProfileByIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var cachedUserProfile = await cache.GetStringAsync(userId.ToString(), cancellationToken);
            if (!string.IsNullOrEmpty(cachedUserProfile)) {
                logger.LogInformation("UserProfile with id {UserId} found in cache", userId);
                return JsonSerializer.Deserialize<UserProfile>(cachedUserProfile)!;
            }
            logger.LogInformation($"{userId} not found in cache");
            var userProfile = await profileRepository.GetUserProfileByIdAsync(userId, cancellationToken);
            await cache.SetStringAsync(userId.ToString(), JsonSerializer.Serialize(userProfile), options, cancellationToken);
            return userProfile;
        }

        public async Task<IEnumerable<UserProfile>> GetUserProfilesAsync(CancellationToken cancellationToken = default)
        {
            var cachedUserProfiles = await cache.GetStringAsync("UserProfiles", cancellationToken);
            if (!string.IsNullOrEmpty(cachedUserProfiles))
            {
                logger.LogInformation("UserProfiles found in cache");
                return JsonSerializer.Deserialize<IEnumerable<UserProfile>>(cachedUserProfiles)!;
            }
            logger.LogInformation("UserProfiles not found in cache");
            var userProfiles = await profileRepository.GetUserProfilesAsync(cancellationToken);
            await cache.SetStringAsync("UserProfiles", JsonSerializer.Serialize(userProfiles), options, cancellationToken);
            return userProfiles;
        }

        public async Task<GroupInfo> StoreGroupInfo(GroupInfo groupInfo, CancellationToken cancellationToken = default)
        {
            await profileRepository.StoreGroupInfo(groupInfo, cancellationToken);

            await cache.SetStringAsync(groupInfo.Id.ToString(), JsonSerializer.Serialize(groupInfo), options, cancellationToken);

            return groupInfo;
        }

        public async Task<UserProfile> StoreProfile(UserProfile userProfile, CancellationToken cancellationToken = default)
        {
            await profileRepository.StoreProfile(userProfile, cancellationToken);

            await cache.SetStringAsync(userProfile.Id.ToString(), JsonSerializer.Serialize(userProfile), options,cancellationToken);

            return userProfile;
        }

        //subinfo
        public async Task<SubInfo> GetGroupNameAndAvatarByIdAsync(Guid groupId, CancellationToken cancellationToken = default)
        {
            var cachedGroupInfo = await cache.GetStringAsync("sub" + groupId.ToString(), cancellationToken);
            if (cachedGroupInfo != null)
            {
                return JsonSerializer.Deserialize<SubInfo>(cachedGroupInfo)!;
            }
            var groupInfo = await profileRepository.GetGroupNameAndAvatarByIdAsync(groupId,cancellationToken);
            await cache.SetStringAsync("sub" + groupId.ToString(), JsonSerializer.Serialize(groupInfo), options, cancellationToken);

            return groupInfo;
        }

        public async Task<SubInfo> GetUserNameAndAvatarByIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var cachedUserProfile = await cache.GetStringAsync("sub" + userId.ToString(), cancellationToken);
            if (cachedUserProfile != null)
            {
                return JsonSerializer.Deserialize<SubInfo>(cachedUserProfile)!;
            }
            logger.LogInformation("Not found name&avatar in cache");
            var userInfo = await profileRepository.GetUserNameAndAvatarByIdAsync(userId, cancellationToken); ;
            await cache.SetStringAsync("sub" + userId.ToString(), JsonSerializer.Serialize(userInfo), options, cancellationToken);

            return userInfo;
        }

        public Task<IEnumerable<SubInfo>> SearchUserByName(string name, CancellationToken cancellationToken = default)
        {
            return profileRepository.SearchUserByName(name, cancellationToken);
        }
    }
}
