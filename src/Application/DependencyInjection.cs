using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddRequestHandlers(Assembly.GetExecutingAssembly());
        return services;
    }
}
