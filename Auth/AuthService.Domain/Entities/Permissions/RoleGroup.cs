using AuthService.Domain.Entities.Groups;
using AuthService.Domain.Enums;
using SocialAppObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Domain.Entities.Permissions
{
    public class RoleGroup : BaseEntity
    {
        public Guid Id { get; set; }
        public GroupRole Role { get; set; }
        public List<RoleGroupPermissionMapping> RoleGroupPermissions { get; set; } = new List<RoleGroupPermissionMapping>();
        public bool IsAllowed { get; set; }
        public Guid? AssociatedGroupId { get; set; }
        public Group Group { get; set; }
    }

}
