using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace BaseTemplate.Application.Common.Events;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDomainEventHandlers(this IServiceCollection services, params Assembly[] assemblies)
    {
        var handlerInterfaceType = typeof(IDomainEventHandler<>);

        var handlers = assemblies
            .SelectMany(x => x.GetTypes())
            .Where(x => !x.IsAbstract && !x.IsInterface)
            .SelectMany(x => x.GetInterfaces(), (type, iface) => new { type, iface })
            .Where(x => x.iface.IsGenericType && x.iface.GetGenericTypeDefinition() == handlerInterfaceType);

        foreach (var handler in handlers)
        {
            services.AddScoped(handler.iface, handler.type);
        }

        return services;
    }
}
