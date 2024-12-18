

namespace AuthService.Domain.Enums
{
    public enum PermissionType
    {
        // Quyền hệ thống chung
        SystemAdminAccess,
        UserManagement,
        ContentModeration,

        // Quyền cấp group
        GroupCreate,
        GroupEdit,
        GroupDelete,
        GroupInviteMember,
        GroupRemoveMember,

        // Quyền nội dung group
        GroupPostCreate,
        GroupPostEdit,
        GroupPostDelete,
        GroupCommentCreate,
        GroupCommentEdit,
        GroupCommentDelete,

        // Quyền quản trị group
        GroupPostPin,
        GroupSettingsModify,
        GroupRoleManage
    }

}
