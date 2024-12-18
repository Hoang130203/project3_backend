using AuthService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.DTOs.Group
{
    public class CreateGroupRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public GroupVisibility Visibility { get; set; }
    }
}
