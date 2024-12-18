using AuthService.Domain.Entities.Groups;
using AuthService.Domain.Entities.Permissions;
using AuthService.Domain.Entities.Users;
using AuthService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Infrastructure.Data.Extensions
{
    internal class InitialData
    {
        public static IEnumerable<User> Users =>
            new List<User>
        {
            new User
            {
                Id =new Guid("58c49479-ec65-4de2-86e7-033c546291aa"),
                Email = "k58a01.mmh@gmail.com",
                Username = "hoangmm",
                PasswordHash = "123456",
                UserType = Domain.Enums.UserType.SystemAdmin,
            },
            new User
            {
                Id=new Guid("58c49479-ec65-4de2-86e7-033c546291ab"),
                Email = "k123@gmail.com",
                Username = "hoang123",
                PasswordHash = "123",
                UserType = Domain.Enums.UserType.RegularUser,
            }
        };

        public static IEnumerable<UserRelationship> UserRelationships =>
            new List<UserRelationship>
            {
                new UserRelationship
                {
                    SourceUserId = new Guid("58c49479-ec65-4de2-86e7-033c546291aa"),
                    TargetUserId = new Guid("58c49479-ec65-4de2-86e7-033c546291ab"),
                    Status = Domain.Enums.ConnectionStatus.Connected,
                    RequestedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                }
            };

        public static IEnumerable<UserProfile> UserProfiles =>
            new List<UserProfile>
            {
                new UserProfile
                {
                    UserId = new Guid("58c49479-ec65-4de2-86e7-033c546291aa"),
                    FullName = "Hoang",
                    ProfilePictureUrl = "https://www.google.com",
                    Website = "https://www.google.com",
                    Bio = "I am a developer",
                    Email = "k58a01.mmh@gmail.com",
                    Phone = "123456789",
                    Location = "Hanoi",
                    DateOfBirth = new DateTime(1999, 1, 1),
                    maritalStatus = Domain.Enums.MaritalStatus.Single
                },
                new UserProfile
                {
                    UserId = new Guid("58c49479-ec65-4de2-86e7-033c546291ab"),
                    FullName = "Hoang",
                    ProfilePictureUrl = "https://www.google.com",
                    Website = "https://www.google.com",
                    Bio = "I am a developer",
                    Email = "kkk2@gmail.com",
                    Phone = "123456789",
                    Location = "Hanoi",
                    DateOfBirth = new DateTime(1999, 1, 1),
                    maritalStatus = Domain.Enums.MaritalStatus.Single
                }

            };
        public static IEnumerable<Group> Groups =>
            new List<Group>
            {
                new Group
                {
                    Id = new Guid("58c49479-ec65-4de2-86e7-033c546291ac"),
                    Name = "Group 1",
                    Description = "Group 1",
                    CreatedAt = DateTime.Now,
                    OwnerId = new Guid("58c49479-ec65-4de2-86e7-033c546291aa"),
                    Visibility = Domain.Enums.GroupVisibility.Public,

                }
            };
        public static IEnumerable<GroupMembership> GroupMemberships =>
            new List<GroupMembership>
            {
                new GroupMembership
                {
                    Id= Guid.NewGuid(),
                    UserId = new Guid("58c49479-ec65-4de2-86e7-033c546291aa"),
                    GroupId = new Guid("58c49479-ec65-4de2-86e7-033c546291ac"),
                    Role = Domain.Enums.GroupRole.Admin,
                    JoinedAt = DateTime.Now
                },
                new GroupMembership
                {
                    UserId = new Guid("58c49479-ec65-4de2-86e7-033c546291ab"),
                    GroupId = new Guid("58c49479-ec65-4de2-86e7-033c546291ac"),
                    Role = Domain.Enums.GroupRole.Member,
                    JoinedAt = DateTime.Now
                }
            };

        public static IEnumerable<Permission> Permissions =>
            Enum.GetValues(typeof(PermissionType))
                .Cast<PermissionType>()
                .Select((type, index) => new Permission
                {
                    Id = GenerateGuidWithIndex(index),
                    Name = $"{type.ToString()}",
                    Description = $"{type.ToString()}",
                    Type = type
                });

        // Hàm tạo GUID dựa trên index
        private static Guid GenerateGuidWithIndex(int index)
        {
            // Sử dụng base GUID làm mẫu
            var baseGuid = Guid.Parse("58c49479-ec65-4de2-86e7-033c54629100");

            // Lấy mảng byte từ base GUID
            var bytes = baseGuid.ToByteArray();

            // Thay đổi 2 byte cuối dựa trên index (nếu nhiều hơn 255 giá trị, sẽ vẫn duy nhất)
            bytes[14] = (byte)((index >> 8) & 0xFF); // Byte cao của index
            bytes[15] = (byte)(index & 0xFF);       // Byte thấp của index

            return new Guid(bytes);
        }

        public static IEnumerable<RoleGroup> RoleGroups =>
            new List<RoleGroup>
            {
                new RoleGroup
                {
                    Id = new Guid("58c49479-ec65-4de2-86e7-033c546291ff"),
                    Role = Domain.Enums.GroupRole.Admin,
                    //Permissions = Permissions.ToList(),
                    IsAllowed = true,
                    AssociatedGroupId = new Guid("58c49479-ec65-4de2-86e7-033c546291ac"),


                },
                new RoleGroup
                {
                    Id =  new Guid("58c49479-ec65-4de2-86e7-033c546291fe"),
                    Role = Domain.Enums.GroupRole.Member,
                    //Permissions = Permissions.Where(p => p.Type == PermissionType.GroupPostCreate).ToList(),
                    IsAllowed = false,
                    AssociatedGroupId  = new Guid("58c49479-ec65-4de2-86e7-033c546291ac"),
                }
            };

        public static IEnumerable<GroupInvitation> GroupInvitations =>
            new List<GroupInvitation>
            {
                new GroupInvitation
                {
                    Id = Guid.NewGuid(),
                    GroupId = new Guid("58c49479-ec65-4de2-86e7-033c546291ac"),
                    InvitedUserId = new Guid("58c49479-ec65-4de2-86e7-033c546291ab"),
                    InviterId = new Guid("58c49479-ec65-4de2-86e7-033c546291aa"),
                    InvitedAt = DateTime.Now,
                    Status = Domain.Enums.InvitationStatus.Pending
                }
            };

        public static IEnumerable<RoleGroupPermissionMapping> RoleGroupPermissionMappings =>
            new List<RoleGroupPermissionMapping>
            { 
                new RoleGroupPermissionMapping
                {
                    PermissionId = new Guid("58C49479-EC65-4DE2-86E7-033C54620000"),
                    RoleGroupId = new Guid("58c49479-ec65-4de2-86e7-033c546291ff")
                },
                new RoleGroupPermissionMapping
                {
                    PermissionId = new Guid("58C49479-EC65-4DE2-86E7-033C54620001"),
                    RoleGroupId = new Guid("58c49479-ec65-4de2-86e7-033c546291ff")
                },
                new RoleGroupPermissionMapping
                {
                    PermissionId = new Guid("58C49479-EC65-4DE2-86E7-033C54620002"),
                    RoleGroupId = new Guid("58c49479-ec65-4de2-86e7-033c546291fe")
                }
            };


    }
}
