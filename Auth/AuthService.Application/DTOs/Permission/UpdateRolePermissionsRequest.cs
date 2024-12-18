using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.DTOs.Permission
{
    public class UpdateRolePermissionsRequest
    {
        public Guid RoleGroupId { get; set; }
        public List<PermissionUpdate> Permissions { get; set; } = new();
    }
}
