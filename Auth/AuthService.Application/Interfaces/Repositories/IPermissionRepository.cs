using AuthService.Domain.Entities.Permissions;
using AuthService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.Interfaces.Repositories
{
    public interface IPermissionRepository
    {
        Task<Permission> GetPermissionByIdAsync(Guid permissionId);
        Task<Permission> GetPermissionByNameAsync(string permissionName);
        Task<bool> HasSystemPermissionAsync(Guid userId, PermissionType permission);
        Task<IEnumerable<RoleGroup>> GetRolePermissionsAsync(GroupRole role, Guid groupId);
        Task<IEnumerable<Permission>> GetPermissionsAsync();
        Task CreatePermissionAsync(Permission permission);
        Task UpdatePermissionAsync(Permission permission);
        Task DeletePermissionAsync(Guid permissionId);
    }

}
