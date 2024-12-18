using AuthService.Domain.Entities.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.Interfaces.Repositories
{

    public interface IUserRelationshipRepository
    {
        Task<UserRelationship> GetUserConnectionByIdAsync(Guid sourceUserId, Guid targetUserId);
        Task CreateUserConnectionAsync(UserRelationship connection);
        Task UpdateUserConnectionAsync(UserRelationship connection);
        Task DeleteUserConnectionAsync(Guid connectionId);
        Task<IEnumerable<UserRelationship>> GetUserConnectionsAsync(Guid userId);
    }
}
