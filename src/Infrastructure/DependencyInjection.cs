﻿using BaseTemplate.Application.Common.Interfaces;
using BaseTemplate.Domain.Constants;
using BaseTemplate.Infrastructure.Data;
using BaseTemplate.Infrastructure.Events;
using BaseTemplate.Infrastructure.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        Guard.Against.Null(connectionString, message: "Connection string 'DefaultConnection' not found.");

        services.AddScoped<IUnitOfWorkFactory>(provider =>
        {
            var config = provider.GetRequiredService<IConfiguration>();
            var user = provider.GetRequiredService<IUser>();
            return new UnitOfWorkFactory(connectionString, user);
        });

        services.AddSingleton(TimeProvider.System);
        services.AddTransient<IIdentityService, IdentityService>();

        using var connection = new SqlConnection(connectionString);
        DatabaseInitializer.Migrate(connection);
        connection.Dispose();

        services.AddSingleton<InMemoryDomainEventWorker>();
        services.AddSingleton<IDomainEventQueue>(sp => sp.GetRequiredService<InMemoryDomainEventWorker>());
        services.AddSingleton<IDomainEventDispatcher, QueuedDomainEventDispatcher>();
        services.AddHostedService(sp => sp.GetRequiredService<InMemoryDomainEventWorker>());
        services.AddAuthorizationCore(options =>
            options.AddPolicy(Policies.CanPurge, policy => policy.RequireRole(Roles.Administrator)));
        return services;
    }
}
