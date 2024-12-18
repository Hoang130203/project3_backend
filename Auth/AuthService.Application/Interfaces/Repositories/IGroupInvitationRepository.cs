using AuthService.Domain.Entities.Groups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.Interfaces.Repositories
{
    public interface IGroupInvitationRepository
    {
        Task<GroupInvitation> GetGroupInvitationByIdAsync(Guid invitationId);
        Task<IEnumerable<GroupInvitation>> GetGroupInvitationsAsync(Guid groupId);
        Task CreateGroupInvitationAsync(GroupInvitation invitation);
        Task UpdateGroupInvitationAsync(GroupInvitation invitation);
        Task DeleteGroupInvitationAsync(Guid invitationId);
    }
}
