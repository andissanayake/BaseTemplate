namespace BaseTemplate.Application.Common.RequestHandler.Behaviors;
using System.Reflection;
using BaseTemplate.Application.Common.Interfaces;
using BaseTemplate.Application.Common.Security;
using Microsoft.Extensions.DependencyInjection;

public class AuthorizationBehavior : IMediatorBehavior
{
    public async Task<Result> HandleAsync<TResponse>(MediatorContext<TResponse> context)
    {
        var identityService = context.ServiceProvider.GetRequiredService<IIdentityService>();

        // Check tenant access for BaseTenantRequest
        if (context.Request is BaseTenantRequest<TResponse> baseTenantRequest)
        {
            if (!await identityService.HasTenantAccessAsync(baseTenantRequest.TenantId))
            {
                return Result.Unauthorized("User doesn't have access to this tenant.");
            }
        }

        // Check authorization attributes
        var authorizeAttributes = context.RequestType.GetCustomAttributes<AuthorizeAttribute>(true);

        foreach (var authorizeAttribute in authorizeAttributes)
        {
            var authResult = await AuthorizeAttributeAsync(authorizeAttribute, identityService);
            if (!ResultCodeMapper.IsSuccess(authResult.Code))
            {
                return authResult;
            }
        }

        return Result.Success();
    }

    private async Task<Result> AuthorizeAttributeAsync(AuthorizeAttribute authorizeAttribute, IIdentityService identityService)
    {
        if (!identityService.IsAuthenticated)
        {
            return Result.Unauthorized("User is not authenticated.");
        }

        // Check roles
        if (!string.IsNullOrWhiteSpace(authorizeAttribute.Roles))
        {
            var hasRole = await CheckRolesAsync(authorizeAttribute.Roles, identityService);
            if (!hasRole)
            {
                return Result.Forbidden("User is not in the required role(s).");
            }
        }

        // Check policy
        if (!string.IsNullOrWhiteSpace(authorizeAttribute.Policy))
        {
            if (!await identityService.AuthorizeAsync(authorizeAttribute.Policy))
            {
                return Result.Forbidden("User does not meet the policy requirements.");
            }
        }

        return Result.Success();
    }

    private async Task<bool> CheckRolesAsync(string roles, IIdentityService identityService)
    {
        var roleArray = roles.Split(',');
        foreach (var role in roleArray)
        {
            if (await identityService.IsInRoleAsync(role.Trim()))
            {
                return true;
            }
        }
        return false;
    }
} 