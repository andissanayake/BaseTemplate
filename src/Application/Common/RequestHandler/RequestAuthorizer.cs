namespace BaseTemplate.Application.Common.RequestHandler;
using System.Reflection;
using BaseTemplate.Application.Common.Interfaces;
using BaseTemplate.Application.Common.Security;

public class RequestAuthorizer : IRequestAuthorizer
{
    private readonly IIdentityService _identityService;

    public RequestAuthorizer(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Result> AuthorizeAsync<TResponse>(IRequest<TResponse> request, Type requestType)
    {
        // Check tenant access for BaseTenantRequest
        if (request is BaseTenantRequest<TResponse> baseTenantRequest)
        {
            if (!await _identityService.HasTenantAccessAsync(baseTenantRequest.TenantId))
            {
                return Result.Unauthorized("User doesn't have access to this tenant.");
            }
        }

        // Check authorization attributes
        var authorizeAttributes = requestType.GetCustomAttributes<AuthorizeAttribute>(true);

        foreach (var authorizeAttribute in authorizeAttributes)
        {
            var authResult = await AuthorizeAttributeAsync(authorizeAttribute);
            if (!ResultCodeMapper.IsSuccess(authResult.Code))
            {
                return authResult;
            }
        }

        return Result.Success();
    }

    private async Task<Result> AuthorizeAttributeAsync(AuthorizeAttribute authorizeAttribute)
    {
        if (!_identityService.IsAuthenticated)
        {
            return Result.Unauthorized("User is not authenticated.");
        }

        // Check roles
        if (!string.IsNullOrWhiteSpace(authorizeAttribute.Roles))
        {
            var hasRole = await CheckRolesAsync(authorizeAttribute.Roles);
            if (!hasRole)
            {
                return Result.Forbidden("User is not in the required role(s).");
            }
        }

        return Result.Success();
    }

    private async Task<bool> CheckRolesAsync(string roles)
    {
        var roleArray = roles.Split(',');
        foreach (var role in roleArray)
        {
            if (await _identityService.IsInRoleAsync(role.Trim()))
            {
                return true;
            }
        }
        return false;
    }
}
