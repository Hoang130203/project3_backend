using MongoDB.Driver;
using ProfileService.Infrastructure;
using ProfileService.Models;
using SocialAppObjects;
using System.Collections.Generic;

namespace ProfileService.Data
{
    public class ProfileRepository : IProfileRepository
    {
        private readonly IMongoCollection<UserProfile> _userProfile;
        private readonly IMongoCollection<GroupInfo> _groupInfo;

        public ProfileRepository(MongoDbClient mongoClient)
        {
            _userProfile = mongoClient.UserProfiles;
            _groupInfo = mongoClient.GroupInfos;
        }
        public async Task<GroupInfo> StoreGroupInfo(GroupInfo groupInfo, CancellationToken cancellationToken = default)
        {
            try
            {
                var filter = Builders<GroupInfo>.Filter.Eq(x => x.Id, groupInfo.Id);
                var options = new ReplaceOptions { IsUpsert = true };

                // Update timestamp
                groupInfo.UpdatedAt = DateTime.UtcNow;
                if (!await _groupInfo.Find(filter).AnyAsync(cancellationToken))
                {
                    groupInfo.CreatedAt = DateTime.UtcNow;
                }

                await _groupInfo.ReplaceOneAsync(
                    filter,
                    groupInfo,
                    options,
                    cancellationToken);

                return groupInfo;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<UserProfile> StoreProfile(UserProfile userProfile, CancellationToken cancellationToken = default)
        {
            var existingProfile = await _userProfile.Find(x => x.Id == userProfile.Id).FirstOrDefaultAsync(cancellationToken: cancellationToken);
            if (existingProfile != null)
            {
                await _userProfile.ReplaceOneAsync(x => x.Id == userProfile.Id, userProfile, cancellationToken: cancellationToken);
            }
            else
            {
                await _userProfile.InsertOneAsync(userProfile, cancellationToken: cancellationToken);
            }

            return userProfile;
        }

        public async Task<bool> DeleteGroupInfo(Guid groupId, CancellationToken cancellationToken = default)
        {
            await _groupInfo.DeleteOneAsync(x => x.Id == groupId, cancellationToken: cancellationToken);
            return true;
        }

        public async Task<bool> DeleteProfile(Guid userId, CancellationToken cancellationToken = default)
        {
            await _userProfile.DeleteOneAsync(x => x.Id == userId, cancellationToken: cancellationToken);
            return true;
        }

        public async Task<GroupInfo> GetGroupInfoByIdAsync(Guid groupId, CancellationToken cancellationToken = default)
        {
            var groupInfo = await _groupInfo.Find(x => x.Id == groupId).FirstOrDefaultAsync(cancellationToken: cancellationToken);

            return groupInfo is null ? throw new Exception("Group not found") : groupInfo;
        }

        public async Task<IEnumerable<GroupInfo>> GetGroupInfosAsync(CancellationToken cancellationToken = default)
        {
            var listGroups =await _groupInfo.Find(x => true).ToListAsync(cancellationToken);
            return listGroups is null ? throw new Exception("Groups not found") : listGroups;
        }

        public async Task<UserProfile> GetUserProfileByIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var userProfile = await _userProfile.Find(x => x.Id == userId).FirstOrDefaultAsync(cancellationToken: cancellationToken);

            return userProfile is null ? throw new Exception("User not found") : userProfile;
        }

        public async Task<IEnumerable<UserProfile>> GetUserProfilesAsync(CancellationToken cancellationToken = default)
        {
            var listUsers = await _userProfile.Find(x => true).ToListAsync(cancellationToken);
            return listUsers is null ? throw new Exception("Users not found") : listUsers;
        }
        //subinfo

        public async Task<SubInfo> GetGroupNameAndAvatarByIdAsync(Guid groupId, CancellationToken cancellationToken = default)
        {
            var groupInfo = await _groupInfo.Find(x => x.Id == groupId).FirstOrDefaultAsync(cancellationToken: cancellationToken);
            if (groupInfo == null)
            {
                // Return default SubInfo instead of throwing error
                return new SubInfo
                {
                    Id = groupId,
                    Name = "Unknown Group",
                    AvatarUrl = "/default-group-avatar.png"
                };
            }
            return new SubInfo
            {
                Id = groupId,
                Name = groupInfo.Name,
                AvatarUrl = groupInfo.GroupPictureUrl
            };
        }

        public async Task<SubInfo> GetUserNameAndAvatarByIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var userProfile = await _userProfile.Find(x => x.Id == userId).FirstOrDefaultAsync(cancellationToken: cancellationToken);
            if (userProfile == null)
            {
                // Return default SubInfo instead of throwing error
                return new SubInfo
                {
                    Id = userId,
                    Name = "Unknown User",
                    AvatarUrl = "/default-avatar.png"
                };
            }
            return new SubInfo
            {
                Id = userId,
                Name = userProfile.FullName,
                AvatarUrl = userProfile.AvatarUrl ?? "/default-avatar.png"
            };
        }

        public Task<IEnumerable<SubInfo>> SearchUserByName(string name, CancellationToken cancellationToken = default)
        {
            var userProfile = _userProfile.Find(x => x.FullName.Contains(name)).ToList();
            return Task.FromResult(userProfile.Select(x => new SubInfo
            {
                Id = x.Id,
                Name = x.FullName,
                AvatarUrl = x.AvatarUrl ?? "/default-avatar.png"
            }));

        }
    }
}
