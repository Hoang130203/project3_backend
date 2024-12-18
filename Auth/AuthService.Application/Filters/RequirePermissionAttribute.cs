using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Threading.Tasks;
using AuthService.Domain.Enums;
using AuthService.Application.Interfaces.Services; // Enums như PermissionType


namespace AuthService.Application.Filters
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class RequirePermissionAttribute : TypeFilterAttribute
    {
        public RequirePermissionAttribute(PermissionType requiredPermission)
            : base(typeof(RequirePermissionFilter))
        {
            Arguments = new object[] { requiredPermission };
        }
    }
    public class RequirePermissionFilter : IAsyncActionFilter
    {
        private readonly PermissionType _requiredPermission;
        private readonly IAuthorizationService _authorizationService;

        public RequirePermissionFilter(PermissionType requiredPermission, IAuthorizationService authorizationService)
        {
            _requiredPermission = requiredPermission;
            _authorizationService = authorizationService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var userIdClaim = context.HttpContext.User.FindFirst("sub");
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            Guid groupId = Guid.Empty;
            if (context.ActionArguments.ContainsKey("groupId"))
            {
                groupId = (Guid)context.ActionArguments["groupId"];
            }

            var hasPermission = groupId == Guid.Empty
                ? await _authorizationService.AuthorizeSystemAccessAsync(userId, _requiredPermission)
                : await _authorizationService.AuthorizeGroupAccessAsync(userId, groupId, _requiredPermission);

            if (!hasPermission)
            {
                context.Result = new ForbidResult();
                return;
            }

            await next();
        }
    }

}
