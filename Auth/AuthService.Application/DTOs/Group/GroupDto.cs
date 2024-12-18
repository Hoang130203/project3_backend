using AuthService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.DTOs.Group
{
    public class GroupDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid CreatorId { get; set; }
        public GroupVisibility Visibility { get; set; }
        public List<GroupMembershipDto> Memberships { get; set; }
    }
}
