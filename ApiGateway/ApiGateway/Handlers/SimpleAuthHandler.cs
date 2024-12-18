using ApiGateway.Proto;
using ApiGateway.Services;
using Microsoft.Extensions.Caching.Memory;
using System.Net;

namespace ApiGateway.Handlers
{
    public class SimpleAuthHandler : IMiddleware
    {
        private readonly IAuthGrpcClient _authClient;
        private readonly IMemoryCache _cache;
        private readonly ILogger<SimpleAuthHandler> _logger;

        public SimpleAuthHandler(
            IAuthGrpcClient authClient,
            IMemoryCache cache,
            ILogger<SimpleAuthHandler> logger)
        {
            _authClient = authClient;
            _cache = cache;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            // Log all incoming requests
            _logger.LogInformation(
                "Incoming request - Method: {Method}, Path: {Path}",
                context.Request.Method,
                context.Request.Path
            );



            var path = context.Request.Path.Value?.TrimStart('/');
            if (path != null && (path.StartsWith("swagger") || path.StartsWith("swagger/v1")))
            {
                await next(context);
                return;
            }

            _logger.LogInformation("Processed path: {Path}", path);

            if (IsAnonymousEndpoint(path))
            {
                _logger.LogInformation("Anonymous endpoint detected: {Path}", path);
                await next(context);
                return;
            }

            if (!context.Request.Headers.TryGetValue("Authorization", out var authHeader) ||
                string.IsNullOrEmpty(authHeader))
            {
                _logger.LogWarning("No Authorization header found");
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            var token = authHeader.ToString().Replace("Bearer ", "");
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("Empty token");
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            var cacheKey = $"token:{token}";
            if (!_cache.TryGetValue<ValidateTokenResponse>(cacheKey, out var userInfo))
            {
                try
                {
                    userInfo = await _authClient.ValidateTokenAsync(token);
                    if (userInfo.IsValid)
                    {
                        _cache.Set(cacheKey, userInfo, TimeSpan.FromMinutes(5));
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error validating token");
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    return;
                }
            }

            if (!userInfo.IsValid)
            {
                _logger.LogWarning("Invalid token");
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            context.Request.Headers.Add("X-UserId", userInfo.UserId);
            context.Request.Headers.Add("X-UserType", userInfo.UserType);

            await next(context);
        }

        private bool IsAnonymousEndpoint(string? path)
        {
            if (string.IsNullOrEmpty(path)) return false;

            _logger.LogInformation("Checking if path is anonymous: {Path}", path);

            var anonymousPaths = new[]
            {
                "auth/users/login",
                "/",
                "files",
                "auth/users/register",
                "health"
            };

            var isAnonymous = anonymousPaths.Any(p =>
                path.StartsWith(p, StringComparison.OrdinalIgnoreCase));

            _logger.LogInformation(
                "Path {Path} is {Result} endpoint",
                path,
                isAnonymous ? "an anonymous" : "a protected"
            );

            return isAnonymous;
        }
    }
}
