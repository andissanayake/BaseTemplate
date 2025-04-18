using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace MediatorS
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRequestHandlers(this IServiceCollection services, Assembly assembly)
        {
            services.AddScoped<IMediator, Mediator>();
            var handlerTypes = assembly.GetTypes()
                .Where(t => !t.IsAbstract && !t.IsInterface)
                .SelectMany(t => t.GetInterfaces(), (t, i) => new { t, i })
                .Where(x =>
                    x.i.IsGenericType &&
                    x.i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>))
                .ToList();

            foreach (var handler in handlerTypes)
            {
                services.AddScoped(handler.i, handler.t);
            }

            return services;
        }
    }

}
