using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddRequestHandlers(Assembly.GetExecutingAssembly());

        // Add Authorization with policies
        services.AddAuthorizationCore(options =>
        {
            AuthorizationConfiguration.ConfigureAuthorization(options);
        });

        return services;
    }
}
