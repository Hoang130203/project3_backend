using AuthService.Application.Data;
using AuthService.Application.DTOs.Permission;
using AuthService.Application.Interfaces.Services;
using AuthService.Domain.Entities.Permissions;
using AuthService.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GroupPermissionsController : ControllerBase
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IAuthorizationService _authService;
        private readonly ILogger<GroupPermissionsController> _logger;

        public GroupPermissionsController(
            IApplicationDbContext dbContext,
            IAuthorizationService authService,
            ILogger<GroupPermissionsController> logger)
        {
            _dbContext = dbContext;
            _authService = authService;
            _logger = logger;
        }

        [HttpGet("group/{groupId}/roles")]
        public async Task<ActionResult<GroupRolePermissionsResponse>> GetGroupRolePermissions(Guid groupId)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null) return Unauthorized();

                // Verify user has permission to manage roles
                var hasPermission = await _authService.AuthorizeGroupAccessAsync(
                    Guid.Parse(userId),
                    groupId,
                    PermissionType.GroupRoleManage);

                if (!hasPermission) return Forbid();

                // Get group details
                var group = await _dbContext.Groups
                    .FirstOrDefaultAsync(g => g.Id == groupId);

                if (group == null) return NotFound("Group not found");

                // Get all role groups for this group
                var roleGroups = await _dbContext.RoleGroups
                    .Include(rg => rg.RoleGroupPermissions)
                        .ThenInclude(rgp => rgp.Permission)
                    .Where(rg => rg.AssociatedGroupId == groupId)
                    .ToListAsync();

                var response = new GroupRolePermissionsResponse
                {
                    GroupId = group.Id,
                    GroupName = group.Name,
                    RolePermissions = roleGroups.Select(rg => new RolePermissionInfo
                    {
                        RoleGroupId = rg.Id,
                        Role = rg.Role,
                        Permissions = rg.RoleGroupPermissions.Select(rgp => new PermissionInfo
                        {
                            Id = rgp.Permission.Id,
                            Name = rgp.Permission.Name,
                            Description = rgp.Permission.Description,
                            Type = rgp.Permission.Type,
                            IsAllowed = rg.IsAllowed
                        }).ToList()
                    }).ToList()
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting role permissions for group {GroupId}", groupId);
                return StatusCode(500, "An error occurred while retrieving group permissions");
            }
        }


        [HttpPut("group/{groupId}/roles")]
        public async Task<IActionResult> UpdateRolePermissions(
    Guid groupId,
    [FromBody] UpdateRolePermissionsRequest request)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null) return Unauthorized();

                // Check if user is group creator or admin
                var membership = await _dbContext.GroupMembership
                    .FirstOrDefaultAsync(m => m.UserId == Guid.Parse(userId) && m.GroupId == groupId);

                if (membership == null ||
                    (membership.Role != GroupRole.Creator && membership.Role != GroupRole.Admin))
                {
                    return Forbid();
                }

                var roleGroup = await _dbContext.RoleGroups
                    .Include(rg => rg.RoleGroupPermissions)
                        .ThenInclude(rgp => rgp.Permission)
                    .FirstOrDefaultAsync(rg =>
                        rg.Id == request.RoleGroupId &&
                        rg.AssociatedGroupId == groupId);

                if (roleGroup == null)
                    return NotFound("Role group not found");

                if (roleGroup.Role == GroupRole.Creator)
                    return BadRequest("Cannot modify Creator role permissions");

                // Process each permission update
                foreach (var permUpdate in request.Permissions)
                {
                    var existingMapping = roleGroup.RoleGroupPermissions
                        .FirstOrDefault(rgp => rgp.Permission.Id == permUpdate.PermissionId);

                    if (permUpdate.IsAllowed)
                    {
                        // Add permission if it doesn't exist
                        if (existingMapping == null)
                        {
                            var permission = await _dbContext.Permissions
                                .FindAsync(permUpdate.PermissionId);

                            if (permission != null)
                            {
                                var newMapping = new RoleGroupPermissionMapping
                                {
                                    RoleGroupId = roleGroup.Id,
                                    PermissionId = permission.Id,
                                    Permission = permission,
                                    UpdatedAt = DateTime.UtcNow
                                };
                                roleGroup.RoleGroupPermissions.Add(newMapping);
                            }
                        }
                    }
                    else
                    {
                        // Remove permission if it exists
                        if (existingMapping != null)
                        {
                            roleGroup.RoleGroupPermissions.Remove(existingMapping);
                        }
                    }
                }

                await _dbContext.SaveChangesAsync(CancellationToken.None);

                _logger.LogInformation(
                    "Updated role permissions for group {GroupId}, role {Role}. ProcessedUpdates: {Count}",
                    groupId,
                    roleGroup.Role,
                    request.Permissions.Count
                );

                // Return updated permissions
                return Ok(new
                {
                    roleGroup.Id,
                    roleGroup.Role,
                    Permissions = roleGroup.RoleGroupPermissions.Select(rgp => new
                    {
                        rgp.Permission.Id,
                        rgp.Permission.Name,
                        rgp.Permission.Type,
                        IsAllowed = true
                    }).ToList()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating role permissions for group {GroupId}", groupId);
                return StatusCode(500, "An error occurred while updating group permissions");
            }
        }

        private string? GetCurrentUserId()
        {
            return HttpContext.Request.Headers["X-UserId"].FirstOrDefault();
        }
    }
    public class UpdateRolePermissionsRequest
    {
        public Guid RoleGroupId { get; set; }
        public List<PermissionUpdate> Permissions { get; set; } = new();
    }

    public class PermissionUpdate
    {
        public Guid PermissionId { get; set; }
        public bool IsAllowed { get; set; }
    }
}
