using AuthService.Application.Interfaces.Repositories;
using AuthService.Domain.Entities.Permissions;
using AuthService.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.Services
{
    public class PermissionService(IApplicationDbContext dbContext) : IPermissionRepository
    {

        public Task CreatePermissionAsync(Permission permission)
        {
            throw new NotImplementedException();
        }

        public Task DeletePermissionAsync(Guid permissionId)
        {
            throw new NotImplementedException();
        }

        public Task<Permission> GetPermissionByIdAsync(Guid permissionId)
        {
            throw new NotImplementedException();
        }

        public Task<Permission> GetPermissionByNameAsync(string permissionName)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Permission>> GetPermissionsAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<RoleGroup>> GetRolePermissionsAsync(GroupRole role, Guid groupId)
        {
            var roleGroup = await dbContext.RoleGroups
                 .Include(rg => rg.RoleGroupPermissions)
                    .ThenInclude(rgp => rgp.Permission)
                .Where(x => x.Role == role && x.AssociatedGroupId == groupId).ToListAsync();
            Console.WriteLine(roleGroup.ToString());
            return roleGroup;
  
        }

        public async Task<bool> HasSystemPermissionAsync(Guid userId, PermissionType permission)
        {
            var user = await dbContext.Users.FindAsync(userId);
            if (user == null)
            {
                throw new Exception("User not found");
            }
            var role = user.GetType;
            var listPermissions = new List<PermissionType>();
            if(role.Equals (UserType.SystemAdmin))
            {
                listPermissions.Add(PermissionType.SystemAdminAccess);
                listPermissions.Add(PermissionType.ContentModeration);
                listPermissions.Add(PermissionType.UserManagement);
            }

            return listPermissions.Contains(permission);
        }

        public Task UpdatePermissionAsync(Permission permission)
        {
            throw new NotImplementedException();
        }
    }
}
