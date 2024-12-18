using AuthService.Domain.Entities.Users;
using AuthService.Domain.Enums;
using SocialAppObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Domain.Entities.Groups
{
    public class GroupMembership : BaseEntity
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public Guid GroupId { get; set; }
        public Group Group { get; set; }
        public GroupRole Role { get; set; }
        public DateTime JoinedAt { get; set; }
    }
}
