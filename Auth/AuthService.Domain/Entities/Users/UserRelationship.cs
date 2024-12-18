using AuthService.Domain.Enums;
using SocialAppObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Domain.Entities.Users
{
    public class UserRelationship : BaseEntity
    {
        public Guid Id { get; set; }
        public Guid SourceUserId { get; set; }
        public User SourceUser { get; set; }
        public Guid TargetUserId { get; set; }
        public User TargetUser { get; set; }
        public ConnectionStatus Status { get; set; }
        public DateTime RequestedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
