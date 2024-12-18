using AuthService.Domain.Entities.Groups;
using AuthService.Domain.Entities.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetUserByIdAsync(Guid userId);
        Task<User> GetUserByEmailAsync(string email);
        Task<User> GetUserByUsernameAsync(string username);
        Task<IEnumerable<User>> GetFriendsAsync(Guid id);
        Task<IEnumerable<Group>> GetGroupsAsync(Guid id);
        Task CreateUserAsync(User user);
        Task<User> UpdateUserAsync(UserProfile user);
        Task DeleteUserAsync(Guid userId);
        Task<IEnumerable<string>> GetFriendsIdAsync(Guid userId);
        Task<IEnumerable<string>> GetGroupsIdAsync(Guid userId);
        Task<IEnumerable<User>> GetFriendSuggestionsAsync(Guid userId, int skip = 0, int take = 20);

    }
}
