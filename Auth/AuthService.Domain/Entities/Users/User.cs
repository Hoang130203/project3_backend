using AuthService.Domain.Entities.Groups;
using AuthService.Domain.Enums;
using JwtConfiguration;
using SocialAppObjects;
//Add-Migration InitialCreate -OutputDir Data/Migrations -Project AuthService.Infrastructure -StartupProject AuthService
namespace AuthService.Domain.Entities.Users
{
    public class User : BaseEntity
    {
        public Guid Id { get; set; }    
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public UserType UserType { get; set; }
        public UserProfile Profile { get; set; }
        public List<UserRelationship> Relationships { get; set; } = new List<UserRelationship>();
        public List<GroupMembership> GroupMemberships { get; set; } = new List<GroupMembership>();

        public bool ValidatePassword(string password, IEncryptor encryptor)
        {
            return PasswordHash == encryptor.GetHash(password, Username);
        }

        public void SetPassword(string password, IEncryptor encryptor)
        {
            PasswordHash = encryptor.GetHash(password, Username);
        }
    }
}

