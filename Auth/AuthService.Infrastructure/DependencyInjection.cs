using AuthService.Application.Data;
using AuthService.Infrastructure.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices
            (this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("Database");
            services.AddScoped<IApplicationDbContext, ApplicationDbContext>();

            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                //options.AddInterceptors(sp.GetRequiredService<CommandInterceptor>());
                options.UseSqlServer(connectionString);
            });

            return services;
        }
    }
}
