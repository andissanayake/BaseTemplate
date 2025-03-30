using BaseTemplate.Domain.Constants;
namespace BaseTemplate.API.Extensions;

public static class AuthorizationExtensions
{
    public static IServiceCollection AddAppAuthorization(this IServiceCollection services)
    {
        services.AddAuthorizationBuilder()
            .AddPolicy(Policies.CanPurge, policy =>
                policy.RequireRole(Roles.Administrator));

        return services;
    }
}
