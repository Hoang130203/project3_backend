using AuthService.Application.CQRS.Commands.Groups.UpdateGroup;
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

namespace AuthService.Application.CQRS.Commands.Groups.UpdateGroup
{
    public class UpdateGroupCommandHandler : ICommandHandler<UpdateGroupCommand, UpdateGroupResult>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly ILogger<UpdateGroupCommandHandler> _logger;
        private readonly IPublishEndpoint _publishEndpoint;

        public UpdateGroupCommandHandler(
            IApplicationDbContext dbContext,
            ILogger<UpdateGroupCommandHandler> logger,
            IPublishEndpoint publishEndpoint)
        {
            _dbContext = dbContext;
            _logger = logger;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<UpdateGroupResult> Handle(UpdateGroupCommand request, CancellationToken cancellationToken)
        {
            // 1. Validate creator's user type
            var user = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);
            var group = await _dbContext.Groups
                .FirstOrDefaultAsync(g => g.Id == request.GroupId, cancellationToken);

            if (user == null)
                throw new InvalidOperationException("User not found");

            if (group == null)
                throw new InvalidOperationException("Group not found");

            var membership = await _dbContext.GroupMembership
          .FirstOrDefaultAsync(m => m.UserId == request.UserId && m.GroupId == request.GroupId, cancellationToken);

            if (membership == null)
                throw new InvalidOperationException("User is not a member of this group");

            var roleGroup = await _dbContext.RoleGroups
                .Include(rg => rg.RoleGroupPermissions)
                    .ThenInclude(rgp => rgp.Permission)
                .FirstOrDefaultAsync(rg =>
                    rg.Role == membership.Role &&
                    rg.AssociatedGroupId == request.GroupId,
                    cancellationToken);

            if (roleGroup == null)
                throw new InvalidOperationException("Role configuration not found");

            var hasEditPermission = roleGroup.RoleGroupPermissions
                .Any(rp => rp.Permission.Type == PermissionType.GroupEdit && roleGroup.IsAllowed);

            if (!hasEditPermission)
                throw new InvalidOperationException("User does not have permission to update group");
            // 3. Update group properties
            if (request.Name != null)
                group.Name = request.Name;

            if (request.Description != null)
                group.Description = request.Description;

            if (request.Visibility != null)
                group.Visibility = request.Visibility;

            await _dbContext.SaveChangesAsync(cancellationToken);

            var eventMessage = new UpdateGroupEvent
            {
                GroupId = group.Id,
                Name = group.Name,
                Description = group.Description,
                OwnerId = group.OwnerId,
                OwnerName = user.Profile?.FullName ?? user.Username,
                UpdatedAt = DateTime.UtcNow,
                Visibility = group.Visibility
            };
            await _publishEndpoint.Publish(eventMessage, cancellationToken);

            _logger.LogInformation(
                "Group {GroupId} updated by user {UserId}. Updated fields: {UpdatedFields}",
                group.Id,
                user.Id,
                GetUpdatedFields(request));


            return new UpdateGroupResult(group.Id);
        }
        private string GetUpdatedFields(UpdateGroupCommand request)
        {
            var updatedFields = new List<string>();
            if (request.Name != null) updatedFields.Add("name");
            if (request.Description != null) updatedFields.Add("description");
            if (request.Visibility !=null) updatedFields.Add("visibility");
            return string.Join(", ", updatedFields);
        }
    }
}
