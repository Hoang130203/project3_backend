using AuthService.Application.DTOs.Group;
using AuthService.Domain.Entities.Groups;
using AuthService.Application.Interfaces.Repositories;
using AuthService.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Application.Services
{
    public class GroupService(IApplicationDbContext dbContext) : IGroupRepository
    {


        public Task CreateGroupAsync(Group group, User user)
        {
            throw new NotImplementedException();
        }

        public Task DeleteGroupAsync(Guid groupId, User user)
        {
            throw new NotImplementedException();
        }

        public Task<Group> GetGroupByIdAsync(Guid groupId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Group>> GetGroupsAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<GroupMembership> GetMembershipAsync(Guid userId, Guid groupId)
        {
            var membership = await dbContext.GroupMembership.Where(x => x.UserId == userId && x.GroupId == groupId).FirstOrDefaultAsync();
            if (membership == null) {
                throw new Exception("Membership not found");
            }
            return membership;
        }

        public Task<IEnumerable<User>> GetUsersByGroupAsync(Guid groupId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateGroupAsync(Group group, User user)
        {
            throw new NotImplementedException();
        }
        public async Task<IEnumerable<Group>> GetGroupsByCreatorAsync(Guid creatorId)
        {
            return await dbContext.Groups
                .Include(g => g.Memberships)
                .Where(g => g.OwnerId == creatorId)
                .ToListAsync();
        }
    }
}
