using BaseTemplate.API.Extensions;
using BaseTemplate.API.Services;
using BaseTemplate.Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;


namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddAPI(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IUser, CurrentUserService>();
        services.AddHttpContextAccessor();
        //services.AddExceptionHandler<CustomExceptionHandler>();
        services.AddHealthChecks();
        services.AddControllers();
        services.Configure<ApiBehaviorOptions>(options =>
            options.SuppressModelStateInvalidFilter = true);

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(config =>
        {
            config.SwaggerDoc("v1", new OpenApiInfo() { Title = "App Api", Version = "v1" });
            config.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "bearer"
            });
            config.AddSecurityRequirement(
                new OpenApiSecurityRequirement{
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type=ReferenceType.SecurityScheme,
                                    Id="Bearer"
                                }
                            },
                            Array.Empty<string>()
                        }
                });
        });

        services.AddApiAuthentication(configuration);
        services.AddAppAuthorization();

        return services;
    }
}
