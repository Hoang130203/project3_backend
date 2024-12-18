using AuthService.Domain.Enums;
using SocialAppObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AuthService.Domain.Entities.Users
{
    public class UserProfile : BaseEntity
    {
        public Guid UserId { get; set; }
        [JsonIgnore]
        public User User { get; set; }
        public string FullName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string ProfilePictureUrl { get; set; } = string.Empty;
        public string ProfileBackgroundUrl { get; set; } = string.Empty;
        public bool IsMale { get; set; } = true;
        public string Bio { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Website { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public MaritalStatus maritalStatus { get; set; }
    }
}
