using BaseTemplate.Application.Common.Interfaces;
using BaseTemplate.Infrastructure.Data;
using BaseTemplate.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BaseTemplate.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")!;
        services.AddSingleton<IDbConnectionFactory>(provider =>
        {
            var config = provider.GetRequiredService<IConfiguration>();
            return new PostgresConnectionFactory(connectionString);
        });

        services.AddDbContext<BaseDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString,
                    b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

        services.AddScoped<IBaseDbContext>(provider => provider.GetRequiredService<BaseDbContext>());

        services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());


        services.AddScoped<IUserProfileService, UserProfileService>();
        services.AddMemoryCache();
        services.AddSingleton(TimeProvider.System);
        return services;
    }
}
