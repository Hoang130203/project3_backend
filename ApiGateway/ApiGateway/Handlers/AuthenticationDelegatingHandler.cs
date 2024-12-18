using ApiGateway.Services;
using Microsoft.Extensions.Caching.Memory;
using System.Net;

namespace ApiGateway.Handlers
{
    public class AuthenticationDelegatingHandler : DelegatingHandler
    {
        private readonly IAuthGrpcClient _authClient;
        private readonly IMemoryCache _cache;
        private readonly ILogger<AuthenticationDelegatingHandler> _logger;
        private readonly IConfiguration _configuration;

        public AuthenticationDelegatingHandler(
            IAuthGrpcClient authClient,
            IMemoryCache cache,
            ILogger<AuthenticationDelegatingHandler> logger,
            IConfiguration configuration)
        {
            _authClient = authClient;
            _cache = cache;
            _logger = logger;
            _configuration = configuration;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            if (IsAnonymousEndpoint(request.RequestUri?.PathAndQuery))
            {
                return await base.SendAsync(request, cancellationToken);
            }

            if (!request.Headers.TryGetValues("Authorization", out var authHeaders))
            {
                return new HttpResponseMessage(HttpStatusCode.Unauthorized);
            }

            var token = authHeaders.FirstOrDefault()?.Replace("Bearer ", "");
            if (string.IsNullOrEmpty(token))
            {
                return new HttpResponseMessage(HttpStatusCode.Unauthorized);
            }

            // Try get validation result from cache
            var cacheKey = $"token:{token}";
            if (!_cache.TryGetValue<Proto.ValidateTokenResponse>(cacheKey, out var validationResult))
            {
                try
                {
                    validationResult = await _authClient.ValidateTokenAsync(token);
                    if (validationResult.IsValid)
                    {
                        _cache.Set(cacheKey, validationResult, TimeSpan.FromMinutes(5));
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to validate token");
                    return new HttpResponseMessage(HttpStatusCode.InternalServerError);
                }
            }

            if (!validationResult.IsValid)
            {
                return new HttpResponseMessage(HttpStatusCode.Unauthorized);
            }

            // Add user info to headers for downstream services
            request.Headers.Add("X-UserId", validationResult.UserId);
            request.Headers.Add("X-UserType", validationResult.UserType);
            //request.Headers.Add("X-UserRoles", validationResult.User.ToArray());

            // Check for required permissions
            var (requiresPermission, permissionType, groupId) = GetRequiredPermission(request);
            if (requiresPermission)
            {
                try
                {
                    var permissionResponse = groupId != null
                        ? await _authClient.CheckGroupPermissionAsync(
                            validationResult.UserId, groupId, permissionType)
                        : await _authClient.CheckSystemPermissionAsync(
                            validationResult.UserId, permissionType);

                    if (!permissionResponse.IsAllowed)
                    {
                        return new HttpResponseMessage(HttpStatusCode.Forbidden);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to check permissions");
                    return new HttpResponseMessage(HttpStatusCode.InternalServerError);
                }
            }

            return await base.SendAsync(request, cancellationToken);
        }

        private bool IsAnonymousEndpoint(string? path)
        {
            if (string.IsNullOrEmpty(path)) return false;

            var anonymousPaths = new[]
            {
            "/auth/login",
            "/auth/register",
            "/health",
            "/swagger"
        };

            return anonymousPaths.Any(p => path.StartsWith(p, StringComparison.OrdinalIgnoreCase));
        }

        private (bool requiresPermission, string permissionType, string? groupId) GetRequiredPermission(HttpRequestMessage request)
        {
            var path = request.RequestUri?.PathAndQuery.ToLower();
            var method = request.Method.ToString().ToUpper();

            // Example permission mapping
            if (path?.StartsWith("/api/posts") == true)
            {
                return (true, "PostManagement", null);
            }

            if (path?.StartsWith("/api/groups/") == true)
            {
                var groupId = ExtractGroupId(path);
                if (groupId != null)
                {
                    return (true, "GroupManagement", groupId);
                }
            }

            return (false, string.Empty, null);
        }

        private string? ExtractGroupId(string path)
        {
            var parts = path.Split('/');
            if (parts.Length >= 4)
            {
                return parts[3];
            }
            return null;
        }
    }
}
