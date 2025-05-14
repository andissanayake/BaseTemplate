using BaseTemplate.Application.Common.Interfaces;
using BaseTemplate.Infrastructure.Data;
using BaseTemplate.Infrastructure.Identity;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
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

        services.AddSingleton(TimeProvider.System);
        services.AddTransient<IIdentityService, IdentityService>();


        // Initialize Logger
        using var loggerFactory = LoggerFactory.Create(builder =>
        {
            //builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Debug);
        });

        // Create a logger instance
        ILogger<DatabaseInitializer> logger = loggerFactory.CreateLogger<DatabaseInitializer>();

        using var connection = new PostgresConnectionFactory(connectionString).CreateConnection();
        new DatabaseInitializer(logger).Migrate(connection, "C:\\Users\\Acer\\source\\test\\BaseTemplate\\src\\Infrastructure\\Data\\Scripts");
        connection.Dispose();
        return services;
    }
}
