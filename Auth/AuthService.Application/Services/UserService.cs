using AuthService.Application.Interfaces.Repositories;
using AuthService.Domain.Entities.Groups;
using AuthService.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.Services
{
    public class UserService(IApplicationDbContext dbContext) : IUserRepository
    {
        public Task CreateUserAsync(User user)
        {
            throw new NotImplementedException();
        }

        public Task DeleteUserAsync(Guid userId)
        {
            throw new NotImplementedException();
        }
        
        public async Task<IEnumerable<User>> GetFriendsAsync(Guid id)
        {
            var friends = await dbContext.UserRelationships
               .Include(r => r.SourceUser.Profile)
               .Include(r => r.TargetUser.Profile)
               .Where(r => r.SourceUserId == id || r.TargetUserId == id)
               .Select(r => r.SourceUserId == id ? r.TargetUser : r.SourceUser)
               .ToListAsync();

            return friends.AsEnumerable();
        }

        public async Task<IEnumerable<string>> GetFriendsIdAsync(Guid userId)
        {
            var friends = await dbContext.UserRelationships
                .Where(r => r.SourceUserId == userId || r.TargetUserId == userId)
                .Select(r => r.SourceUserId == userId ? r.TargetUserId.ToString() : r.SourceUserId.ToString())
                .ToListAsync();
            return friends.AsEnumerable();
        }

        public async Task<IEnumerable<Group>> GetGroupsAsync(Guid id)
        {
            var groups = await dbContext.GroupMembership
                .Include(m => m.Group)
                .Where(m => m.UserId == id)
                .Select(m => m.Group)
                .ToListAsync();
            return groups.AsEnumerable();
        }

        public async Task<IEnumerable<string>> GetGroupsIdAsync(Guid userId)
        {
            var groups =await dbContext.GroupMembership
                .Where(m => m.UserId == userId)
                .Select(m => m.GroupId.ToString()).ToListAsync();
            return groups.AsEnumerable();
        }

        public Task<User> GetUserByEmailAsync(string email)
        {
            throw new NotImplementedException();
        }

        public Task<User> GetUserByIdAsync(Guid userId)
        {
            return dbContext.Users
                .Include(u => u.Profile)
                .FirstOrDefaultAsync(u => u.Id == userId)!;
        }

        public Task<User> GetUserByUsernameAsync(string username)
        {
            throw new NotImplementedException();
        }


        public Task<User> UpdateUserAsync(UserProfile user)
        {
            throw new NotImplementedException();
        }
        public async Task<IEnumerable<User>> GetFriendSuggestionsAsync(Guid userId, int skip = 0, int take = 20)
        {
            try
            {
                // Get existing connections (friends and blocked)
                var existingConnections = await dbContext.UserRelationships
                    .Where(r => r.SourceUserId == userId || r.TargetUserId == userId)
                    .ToListAsync();

                // Get IDs of users to exclude (current friends and blocked users)
                var excludeUserIds = existingConnections
                    .SelectMany(r => new[] { r.SourceUserId, r.TargetUserId })
                    .Distinct()
                    .ToList();

                // Add the user's own ID to exclude
                excludeUserIds.Add(userId);

                // Get suggested users
                var suggestedUsers = await dbContext.Users
                    .Include(u => u.Profile)
                    .Where(u => !excludeUserIds.Contains(u.Id))
                    .OrderBy(u => Guid.NewGuid()) // Random ordering
                    .Skip(skip)
                    .Take(take)
                    .ToListAsync();

                return suggestedUsers;
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting friend suggestions", ex);
            }
        }


    }
}
