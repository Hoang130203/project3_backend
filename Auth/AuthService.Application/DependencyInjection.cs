
using AuthService.Application.Filters;
using AuthService.Application.Interfaces.Repositories;
using AuthService.Application.Interfaces.Services;
using AuthService.Application.Services;
using BuildingBlocks.Behaviors;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BuildingBlocks.Messaging.MassTransit;
using System.Reflection;


namespace AuthService.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices
       (this IServiceCollection services, IConfiguration configuration)
        {
            //services.AddScoped<RequirePermissionFilter>();
            services.AddScoped<IUserRepository, UserService>();
            services.AddScoped<IGroupRepository, GroupService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IAuthorizationService, AuthorizationService>();
            services.AddScoped<IUserRelationshipRepository, UserRelationshipService>();
            services.AddScoped<IPermissionRepository, PermissionService>();
            services.AddScoped<IGroupInvitationRepository, GroupInvitationService>();
            services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                config.AddOpenBehavior(typeof(ValidationBehavior<,>));
                config.AddOpenBehavior(typeof(LoggingBehavior<,>));
            });

            //services.AddFeatureManagement();
            services.AddMessageBroker(configuration, Assembly.GetExecutingAssembly());

            return services;
        }

    }
}
