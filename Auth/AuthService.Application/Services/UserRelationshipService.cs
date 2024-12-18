using AuthService.Application.Interfaces.Repositories;
using AuthService.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.Services
{
    public class UserRelationshipService : IUserRelationshipRepository
    {
        private readonly IApplicationDbContext _dbContext;

        public UserRelationshipService(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task CreateUserConnectionAsync(UserRelationship connection)
        {
            await _dbContext.UserRelationships.AddAsync(connection);
            await _dbContext.SaveChangesAsync(CancellationToken.None);
        }

        public Task DeleteUserConnectionAsync(Guid connectionId)
        {
            var connection = _dbContext.UserRelationships.Find(connectionId);
            if (connection != null)
            {
                _dbContext.UserRelationships.Remove(connection);
                return _dbContext.SaveChangesAsync(CancellationToken.None);
            }
            return Task.CompletedTask;
        }

        public async Task<UserRelationship> GetUserConnectionByIdAsync(Guid sourceUserId, Guid targetUserId)
        {
            return await _dbContext.UserRelationships
                .FirstOrDefaultAsync(r =>
                    (r.SourceUserId == sourceUserId && r.TargetUserId == targetUserId) ||
                    (r.SourceUserId == targetUserId && r.TargetUserId == sourceUserId));
        }

        public Task<IEnumerable<UserRelationship>> GetUserConnectionsAsync(Guid userId)
        {
            return Task.FromResult(_dbContext.UserRelationships
                .Where(r => r.SourceUserId == userId || r.TargetUserId == userId)
                .AsEnumerable());
        }

        public async Task UpdateUserConnectionAsync(UserRelationship connection)
        {
            var existingConnection = await _dbContext.UserRelationships.FindAsync(connection.Id);
            if (existingConnection != null)
            {
                existingConnection.Status = connection.Status;
                existingConnection.UpdatedAt = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync(CancellationToken.None);
            }

        }
    }
}
