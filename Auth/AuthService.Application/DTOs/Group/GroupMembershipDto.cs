using AuthService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.DTOs.Group
{
    public class GroupMembershipDto
    {
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public GroupRole Role { get; set; }
        public DateTime JoinedAt { get; set; }
    }
}
