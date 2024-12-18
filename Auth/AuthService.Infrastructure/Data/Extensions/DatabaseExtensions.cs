using AuthService.Infrastructure.Data.Configurations;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Infrastructure.Data.Extensions
{
    public static class DatabaseExtensions
    {
        public static async Task InitialiseDatabaseAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            context.Database.MigrateAsync().GetAwaiter().GetResult();

            await SeedAsync(context);
        }

        private static async Task SeedAsync(ApplicationDbContext context)
        {
            //await SeedCustomerAsync(context);
            //await SeedProductAsync(context);
            //await SeedOrdersWithItemsAsync(context);
            await SeedUserAsync(context);
            await SeedGroupAsync(context);  
            await SeedUserProfileAsync(context);
            await SeedUserRelationshipAsync(context);
            await SeedGroupMembershipAsync(context);
            await SeedGroupInvitationAsync(context);
            await SeedPermissionAsync(context);
            await SeedRoleGroupAsync(context);
            await SeedRoleGroupPermissionMappingAsync(context);
        }

        private static async Task SeedRoleGroupPermissionMappingAsync(ApplicationDbContext context)
        {
            if (!await context.RoleGroupPermissionMappings.AnyAsync())
            {
                context.RoleGroupPermissionMappings.AddRange(InitialData.RoleGroupPermissionMappings);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedGroupAsync(ApplicationDbContext context)
        {
            if(!await context.Groups.AnyAsync())
            {
                context.Groups.AddRange(InitialData.Groups);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedUserAsync(ApplicationDbContext context)
        {
            if (!await context.Users.AnyAsync())
            {
                context.Users.AddRange(InitialData.Users);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedUserProfileAsync(ApplicationDbContext context)
        {
            if (!await context.UserProfiles.AnyAsync())
            {
                context.UserProfiles.AddRange(InitialData.UserProfiles);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedUserRelationshipAsync(ApplicationDbContext context)
        {
            if (!await context.UserRelationships.AnyAsync())
            {
                context.UserRelationships.AddRange(InitialData.UserRelationships);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedGroupMembershipAsync(ApplicationDbContext context)
        {
            if (!await context.GroupMembership.AnyAsync())
            {
                context.GroupMembership.AddRange(InitialData.GroupMemberships);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedGroupInvitationAsync(ApplicationDbContext context)
        {
            if (!await context.GroupInvitations.AnyAsync())
            {
                context.GroupInvitations.AddRange(InitialData.GroupInvitations);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedPermissionAsync(ApplicationDbContext context)
        {
            if (!await context.Permissions.AnyAsync())
            {
                context.Permissions.AddRange(InitialData.Permissions);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedRoleGroupAsync(ApplicationDbContext context)
        {
            if (!await context.RoleGroups.AnyAsync())
            {
                context.RoleGroups.AddRange(InitialData.RoleGroups);
                await context.SaveChangesAsync();
            }
        }
    }
}