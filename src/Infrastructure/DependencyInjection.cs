using BaseTemplate.Application.Common.Interfaces;
using BaseTemplate.Infrastructure.Data;
using BaseTemplate.Infrastructure.Services;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        //dapper 
        DefaultTypeMap.MatchNamesWithUnderscores = true;
        SimpleCRUD.SetDialect(SimpleCRUD.Dialect.PostgreSQL);
        SimpleCRUD.SetTableNameResolver(new SnakeCaseTableNameResolver());
        SimpleCRUD.SetColumnNameResolver(new SnakeCaseColumnNameResolver());
        var connectionString = configuration.GetConnectionString("DefaultConnection")!;
        services.AddSingleton<IDbConnectionFactory>(provider =>
        {
            var config = provider.GetRequiredService<IConfiguration>();
            return new PostgresConnectionFactory(connectionString);
        });
        services.AddSingleton<IUnitOfWorkFactory, UnitOfWorkFactory>();
        //end dapper

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

        services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());


        services.AddScoped<IUserProfileService, UserProfileService>();
        services.AddMemoryCache();
        services.AddSingleton(TimeProvider.System);
        services.AddSingleton<DatabaseInitializer>();
        return services;
    }
}
