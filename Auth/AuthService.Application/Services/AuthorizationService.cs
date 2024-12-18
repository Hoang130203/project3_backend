using AuthService.Application.Interfaces.Repositories;
using AuthService.Application.Interfaces.Services;
using AuthService.Domain.Enums;
using Microsoft.Extensions.Logging;


namespace AuthService.Application.Services
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly IUserRepository _userRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly IPermissionRepository _permissionRepository;
        private readonly ILogger<AuthorizationService> _logger;
        public AuthorizationService(
            IUserRepository userRepository,
            IGroupRepository groupRepository,
            IPermissionRepository permissionRepository,
             ILogger<AuthorizationService> logger)
        {
            _userRepository = userRepository;
            _groupRepository = groupRepository;
            _permissionRepository = permissionRepository;
            _logger = logger;

        }

        // Kiểm tra quyền hệ thống
        public async Task<bool> AuthorizeSystemAccessAsync(Guid userId, PermissionType permission)
        {
            // Giả định UserType.SystemAdmin là loại user có quyền hệ thống
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("User {UserId} not found during system access authorization", userId);

                return false;
            }

            // Nếu là Admin toàn hệ thống thì có tất cả quyền
            if (user.UserType == UserType.SystemAdmin)
            {
                return true;
            }

            // Hoặc kiểm tra quyền cụ thể
            var hasPermission = await _permissionRepository.HasSystemPermissionAsync(userId, permission);
            return hasPermission;
        }

        // Kiểm tra quyền trong nhóm
        public async Task<bool> AuthorizeGroupAccessAsync(Guid userId, Guid groupId, PermissionType permission)
        {
            var membership = await _groupRepository.GetMembershipAsync(userId, groupId);
            if (membership == null) return false;

            // Lấy role của user trong nhóm
            var role = membership.Role;

            // Kiểm tra quyền dựa trên role
            var RoleGroups = await _permissionRepository.GetRolePermissionsAsync(role, groupId);
            Console.WriteLine(RoleGroups.First().RoleGroupPermissions.First().Permission.Type.ToString());
            //return true;
            return RoleGroups.Any(mapping =>
                    mapping.RoleGroupPermissions.Any(p => p.Permission.Type == permission) && mapping.IsAllowed);
        }
        public async Task<bool> AuthorizeMultiplePermissionsAsync(
        Guid userId,
        Guid? groupId,
        IEnumerable<PermissionType> permissions)
        {
            foreach (var permission in permissions)
            {
                var isAuthorized = groupId.HasValue
                    ? await AuthorizeGroupAccessAsync(userId, groupId.Value, permission)
                    : await AuthorizeSystemAccessAsync(userId, permission);

                if (!isAuthorized)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
