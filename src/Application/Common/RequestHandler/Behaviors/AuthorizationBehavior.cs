namespace BaseTemplate.Application.Common.RequestHandler.Behaviors;
using System.Reflection;
using BaseTemplate.Application.Common.Interfaces;
using BaseTemplate.Application.Common.Security;
using Microsoft.Extensions.DependencyInjection;

public class AuthorizationBehavior : IMediatorBehavior
{
    public async Task<Result> HandleAsync<TResponse>(MediatorContext<TResponse> context)
    {
        var userProfileService = context.ServiceProvider.GetRequiredService<IUserTenantProfileService>();
        var authorizeAttributes = context.RequestType.GetCustomAttributes<AuthorizeAttribute>(true);

        foreach (var authorizeAttribute in authorizeAttributes)
        {
            var authResult = await AuthorizeAttributeAsync(authorizeAttribute, userProfileService);
            if (!ResultCodeMapper.IsSuccess(authResult.Code))
            {
                return authResult;
            }
        }

        return Result.Success();
    }

    private async Task<Result> AuthorizeAttributeAsync(AuthorizeAttribute authorizeAttribute, IUserTenantProfileService userProfileService)
    {
        if (!string.IsNullOrWhiteSpace(authorizeAttribute.Roles))
        {
            var userInfo = await userProfileService.GetUserProfileAsync();
            if (!userInfo!.Roles.Any())
            {
                return Result.Unauthorized("User is not authenticated.");
            }

            var hasRole = CheckRolesAsync(authorizeAttribute.Roles, userInfo.Roles);
            if (!hasRole)
            {
                return Result.Forbidden("User is not in the required role(s).");
            }
        }

        return Result.Success();
    }

    private bool CheckRolesAsync(string roles, List<string> userRoles)
    {
        var roleArray = roles.Split(',');
        foreach (var role in roleArray)
        {
            if (userRoles.Any(r => r == role))
            {
                return true;
            }
        }
        return false;
    }
}
