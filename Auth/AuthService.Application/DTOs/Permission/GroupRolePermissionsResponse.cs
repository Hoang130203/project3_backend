using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.DTOs.Permission
{
    public class GroupRolePermissionsResponse
    {
        public Guid GroupId { get; set; }
        public string GroupName { get; set; }
        public List<RolePermissionInfo> RolePermissions { get; set; } = new();
    }
}
