namespace BaseTemplate.Application.Common.RequestHandler;
using System.Reflection;
using BaseTemplate.Application.Common.Interfaces;
using BaseTemplate.Application.Common.Security;

public class RequestAuthorizer : IRequestAuthorizer
{
    private readonly IUser _user;

    public RequestAuthorizer(IUser user)
    {
        _user = user;
    }

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

    private async Task<Result> AuthorizeAttributeAsync(AuthorizeAttribute authorizeAttribute)
    {
        if (string.IsNullOrEmpty(_user.Email))
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

        return false;
    }
}
