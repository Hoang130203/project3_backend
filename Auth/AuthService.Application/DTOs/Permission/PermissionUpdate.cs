using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.DTOs.Permission
{
    public class PermissionUpdate
    {
        public Guid PermissionId { get; set; }
        public bool IsAllowed { get; set; }
    }
}
