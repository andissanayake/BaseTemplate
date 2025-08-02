namespace BaseTemplate.Application.Common.RequestHandler;
using System.Reflection;
using BaseTemplate.Application.Common.Interfaces;
using BaseTemplate.Application.Common.Security;

public class RequestAuthorizer(IUser user, IUserProfileService userTenantProfileService) : IRequestAuthorizer
{
    private readonly IUser _user = user;
    private readonly IUserProfileService _tenantProfileService = userTenantProfileService;

    public async Task<Result> AuthorizeAsync<TResponse>(IRequest<TResponse> request, Type requestType)
    {
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

    private Task<Result> AuthorizeAttributeAsync(AuthorizeAttribute authorizeAttribute)
    {
        if (string.IsNullOrEmpty(_user.Email))
        {
            return Task.FromResult(Result.Unauthorized("User is not authenticated."));
        }

        // Check roles
        if (!string.IsNullOrWhiteSpace(authorizeAttribute.Roles))
        {
            var userInfo = _tenantProfileService.UserProfile;
            var hasRole = CheckRoles(authorizeAttribute.Roles, userInfo.Roles);
            if (!hasRole)
            {
                return Task.FromResult(Result.Forbidden("User is not in the required role(s)."));
            }
        }

        return Task.FromResult(Result.Success());
    }

    private static bool CheckRoles(string roles, List<string> currentRoles)
    {
        var roleArray = roles.Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(r => r.Trim())
            .Where(r => !string.IsNullOrEmpty(r))
            .ToList();
        if (currentRoles.Any(r => r == Roles.TenantOwner))
        {
            return true;
        }
        // Check if user has any of the required roles
        return roleArray.Any(requiredRole =>
            currentRoles.Any(userRole =>
                string.Equals(userRole, requiredRole, StringComparison.OrdinalIgnoreCase)));
    }
}
