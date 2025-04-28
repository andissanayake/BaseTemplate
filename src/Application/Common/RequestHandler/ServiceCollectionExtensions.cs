using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace BaseTemplate.Application.Common.RequestHandler;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRequestHandlers(this IServiceCollection services, Assembly assembly)
    {
        services.AddScoped<IMediator, Mediator>();

        var handlerTypes = assembly.GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface)
            .SelectMany(t => t.GetInterfaces(), (t, i) => new { Implementation = t, Interface = i })
            .Where(x =>
                x.Interface.IsGenericType &&
                x.Interface.GetGenericTypeDefinition() == typeof(IRequestHandler<,>))
            .ToList();

        foreach (var handler in handlerTypes.Distinct())
        {
            services.AddScoped(handler.Interface, handler.Implementation);
        }

        return services;
    }
}
