using BaseTemplate.API.Infrastructure;
using BaseTemplate.API.Services;
using BaseTemplate.Application.Common.Interfaces;
using BaseTemplate.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;


namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddAPI(this IServiceCollection services)
    {
        services.AddDatabaseDeveloperPageExceptionFilter();
        services.AddSingleton<IUser, CurrentUserService>();
        services.AddHttpContextAccessor();
        services.AddExceptionHandler<CustomExceptionHandler>();
        services.AddHealthChecks()
            .AddDbContextCheck<ApplicationDbContext>();
        services.AddControllers();
        services.Configure<ApiBehaviorOptions>(options =>
            options.SuppressModelStateInvalidFilter = true);

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        return services;
    }
}
