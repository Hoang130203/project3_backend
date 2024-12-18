using AuthService.Domain.Enums;
using SocialAppObjects;
using AuthService.Domain.Entities.Users;
using System.Text.Json.Serialization;

namespace AuthService.Domain.Entities.Groups
{
    public class Group : BaseEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid OwnerId { get; set; }
        public User? Owner { get; set; }
        public DateTime CreatedAt { get; set; }
        public GroupVisibility Visibility { get; set; }
        [JsonIgnore]
        public List<GroupMembership>? Memberships { get; set; } = new List<GroupMembership>();
    }
}
