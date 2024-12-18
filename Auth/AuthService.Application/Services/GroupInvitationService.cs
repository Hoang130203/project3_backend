using AuthService.Application.Interfaces.Repositories;
using AuthService.Domain.Entities.Groups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.Services
{
    public class GroupInvitationService : IGroupInvitationRepository
    {
        public Task CreateGroupInvitationAsync(GroupInvitation invitation)
        {
            throw new NotImplementedException();
        }

        public Task DeleteGroupInvitationAsync(Guid invitationId)
        {
            throw new NotImplementedException();
        }

        public Task<GroupInvitation> GetGroupInvitationByIdAsync(Guid invitationId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<GroupInvitation>> GetGroupInvitationsAsync(Guid groupId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateGroupInvitationAsync(GroupInvitation invitation)
        {
            throw new NotImplementedException();
        }
    }
}
