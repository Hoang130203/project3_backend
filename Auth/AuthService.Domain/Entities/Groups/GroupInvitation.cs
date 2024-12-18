using AuthService.Domain.Entities.Users;
using AuthService.Domain.Enums;
using SocialAppObjects;


namespace AuthService.Domain.Entities.Groups
{
    public class GroupInvitation : BaseEntity
    {
        public Guid Id { get; set; }
        public Guid InviterId { get; set; }
        public User Inviter { get; set; }
        public Guid InvitedUserId { get; set; }
        public User InvitedUser { get; set; }
        public Guid GroupId { get; set; }
        public Group Group { get; set; }
        public DateTime InvitedAt { get; set; }
        public InvitationStatus Status { get; set; }
    }
}
