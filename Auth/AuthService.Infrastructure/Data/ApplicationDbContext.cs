using AuthService.Application.Data;
using AuthService.Domain.Entities.Groups;
using AuthService.Domain.Entities.Permissions;
using AuthService.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<User> Users => Set<User>();
        public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
        public DbSet<UserRelationship> UserRelationships => Set<UserRelationship>();
        public DbSet<Group> Groups => Set<Group>();
        public DbSet<GroupInvitation> GroupInvitations => Set<GroupInvitation>();
        public DbSet<GroupMembership> GroupMembership => Set<GroupMembership>();
        public DbSet<Permission> Permissions => Set<Permission>();
        public DbSet<RoleGroup> RoleGroups => Set<RoleGroup>();

        public DbSet<RoleGroupPermissionMapping> RoleGroupPermissionMappings => Set<RoleGroupPermissionMapping>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Áp dụng tất cả các cấu hình từ assembly
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(modelBuilder);
        }

    }
}
