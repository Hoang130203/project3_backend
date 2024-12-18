using AuthService.Domain.Enums;
using SocialAppObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Domain.Entities.Permissions
{
    public class RoleGroupPermissionMapping : BaseEntity
    {
        public Guid RoleGroupId { get; set; }
        public RoleGroup RoleGroup { get; set; }
        public Guid PermissionId { get; set; }
        public Permission Permission { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
