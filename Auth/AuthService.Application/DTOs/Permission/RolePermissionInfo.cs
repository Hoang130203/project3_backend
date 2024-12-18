using AuthService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.DTOs.Permission
{
    public class RolePermissionInfo
    {
        public Guid RoleGroupId { get; set; }
        public GroupRole Role { get; set; }
        public List<PermissionInfo> Permissions { get; set; } = new();
    }
}
