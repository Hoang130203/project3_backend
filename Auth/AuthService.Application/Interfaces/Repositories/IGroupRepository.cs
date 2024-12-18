using AuthService.Domain.Entities.Groups;
using AuthService.Domain.Entities.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.Interfaces.Repositories
{
    public interface IGroupRepository
    {
        Task<Group> GetGroupByIdAsync(Guid groupId);
        Task<IEnumerable<Group>> GetGroupsAsync();
        Task<IEnumerable<User>> GetUsersByGroupAsync(Guid groupId);
        Task<GroupMembership> GetMembershipAsync(Guid userId, Guid groupId);
        Task CreateGroupAsync(Group group, User user);
        Task UpdateGroupAsync(Group group, User user);
        Task DeleteGroupAsync(Guid groupId, User user);
        Task<IEnumerable<Group>> GetGroupsByCreatorAsync(Guid creatorId);
    }

}
