using AuthService.Domain.Entities.Groups;
using AuthService.Domain.Entities.Permissions;
using AuthService.Domain.Enums;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.CQRS.Commands.Groups.CreateGroup
{
    public class CreateGroupCommandHandler : ICommandHandler<CreateGroupCommand, CreateGroupResult>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ILogger<CreateGroupCommandHandler> _logger;
        private readonly IPublishEndpoint _publishEndpoint;

        public CreateGroupCommandHandler(
            IApplicationDbContext dbContext,
            ILogger<CreateGroupCommandHandler> logger,
            IPublishEndpoint publishEndpoint)
        {
            _dbContext = dbContext;
            _logger = logger;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<CreateGroupResult> Handle(CreateGroupCommand request, CancellationToken cancellationToken)
        {
            // 1. Validate creator's user type
            var creator = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Id == request.CreatorId, cancellationToken);

            if (creator == null)
                throw new InvalidOperationException("User not found");

            if (creator.UserType != UserType.BusinessAccount && creator.UserType != UserType.RegularUser)
                throw new InvalidOperationException("Only business and regular users can create groups");

            // 2. Create the group
            var group = new Group
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                OwnerId = request.CreatorId,
                CreatedAt = DateTime.UtcNow,
                Visibility = request.Visibility
            };

            await _dbContext.Groups.AddAsync(group, cancellationToken);

            // 3. Set up default role permissions
            var roleGroups = new List<RoleGroup>
            {
                // Creator Role
                CreateRoleGroup(group.Id, GroupRole.Creator, new[]
                {

                    // Group Management
                    PermissionType.GroupEdit,
                    PermissionType.GroupDelete,
                    PermissionType.GroupInviteMember,
                    PermissionType.GroupRemoveMember,
                    PermissionType.GroupRoleManage,
                    PermissionType.GroupSettingsModify,
                    // Content Management
                    PermissionType.GroupPostCreate,
                    PermissionType.GroupPostEdit,
                    PermissionType.GroupPostDelete,
                    PermissionType.GroupPostPin,
                    PermissionType.GroupCommentCreate,
                    PermissionType.GroupCommentEdit,
                    PermissionType.GroupCommentDelete
                }),

                // Admin Role
                CreateRoleGroup(group.Id, GroupRole.Admin, new[]
                {
                    // User Management
                    PermissionType.GroupInviteMember,
                    PermissionType.GroupRemoveMember,
                    PermissionType.GroupRoleManage,
                    // Content Management
                    PermissionType.GroupPostCreate,
                    PermissionType.GroupPostEdit,
                    PermissionType.GroupPostDelete,
                    PermissionType.GroupPostPin,
                    PermissionType.GroupCommentCreate,
                    PermissionType.GroupCommentEdit,
                    PermissionType.GroupCommentDelete
                }),

                // Moderator Role
                CreateRoleGroup(group.Id, GroupRole.Moderator, new[]
                {
                    // Content Moderation
                    PermissionType.GroupPostCreate,
                    PermissionType.GroupPostEdit,
                    PermissionType.GroupPostDelete,
                    PermissionType.GroupCommentCreate,
                    PermissionType.GroupCommentEdit,
                    PermissionType.GroupCommentDelete
                }),

                // Member Role
                CreateRoleGroup(group.Id, GroupRole.Member, new[]
                {
                    PermissionType.GroupPostCreate,
                    PermissionType.GroupCommentCreate
                }),

                // Banned Role (No permissions)
                CreateRoleGroup(group.Id, GroupRole.Banned, Array.Empty<PermissionType>())
            };

            await _dbContext.RoleGroups.AddRangeAsync(roleGroups, cancellationToken);

            // 4. Add creator as member with Creator role
            var creatorMembership = new GroupMembership
            {
                Id = Guid.NewGuid(),
                UserId = request.CreatorId,
                GroupId = group.Id,
                Role = GroupRole.Creator,
                JoinedAt = DateTime.UtcNow
            };

            await _dbContext.GroupMembership.AddAsync(creatorMembership, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            // 5. Publish group created event
            var eventMessage = new CreateGroupEvent
            {
                GroupId = group.Id,
                Name = group.Name,
                Description = group.Description,
                OwnerId = group.OwnerId,
                OwnerName = creator.Profile?.FullName ?? creator.Username,
                CreatedAt = group.CreatedAt,
                Visibility = group.Visibility
            };

            await _publishEndpoint.Publish(eventMessage, cancellationToken);

            return new CreateGroupResult(group.Id);
        }

        private RoleGroup CreateRoleGroup(Guid groupId, GroupRole role, PermissionType[] permissions)
        {
            var roleGroup = new RoleGroup
            {
                Id = Guid.NewGuid(),
                Role = role,
                AssociatedGroupId = groupId,
                IsAllowed = true
            };

            roleGroup.RoleGroupPermissions = permissions.Select(p => new RoleGroupPermissionMapping
            {
                RoleGroupId = roleGroup.Id,
                Permission = new Permission
                {
                    Id = Guid.NewGuid(),
                    Type = p,
                    Name = p.ToString(),
                    Description = $"Permission to {p.ToString()}"
                },
                UpdatedAt = DateTime.UtcNow
            }).ToList();

            return roleGroup;
        }
    }
}