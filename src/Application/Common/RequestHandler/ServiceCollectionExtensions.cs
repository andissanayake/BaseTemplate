﻿using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace BaseTemplate.Application.Common.RequestHandler;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRequestHandlers(this IServiceCollection services, Assembly assembly)
    {
        // Register mediator and its dependencies
        services.AddScoped<IRequestValidator, RequestValidator>();
        services.AddScoped<IRequestAuthorizer, RequestAuthorizer>();
        services.AddScoped<IRequestHandlerResolver, RequestHandlerResolver>();
        services.AddScoped<IMediator, Mediator>();

        // Register all request handlers
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
