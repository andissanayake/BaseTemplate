namespace BaseTemplate.Application.Common.RequestHandler;
using BaseTemplate.Application.Common.Security;
using BaseTemplate.Application.Common.Interfaces;
using System.Reflection;

public interface IRequest<TResponse> { }

public interface IRequestHandler<in TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    Task<Result<TResponse>> HandleAsync(TRequest request, CancellationToken cancellationToken);
}

public interface IMediator
{
    Task<Result<TResponse>> SendAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);
}

public class Mediator : IMediator
{
    private readonly IServiceProvider _provider;
    private readonly IIdentityService _identityService;

    public Mediator(IServiceProvider provider, IIdentityService identityService)
    {
        _provider = provider;
        _identityService = identityService;
    }

    public async Task<Result<TResponse>> SendAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        if (request is null)
        {
            return Result<TResponse>.Validation("Request is required", new Dictionary<string, string[]>
            {
                ["request"] = ["Request cannot be null."]
            });
        }

        var requestType = request.GetType();
        var authorizeAttributes = requestType.GetCustomAttributes<AuthorizeAttribute>(true);

        foreach (var authorizeAttribute in authorizeAttributes)
        {
            if (!_identityService.IsAuthenticated)
            {
                return Result<TResponse>.Unauthorized("User is not authenticated.");
            }

            if (!string.IsNullOrWhiteSpace(authorizeAttribute.Roles))
            {
                var authorized = false;
                var roles = authorizeAttribute.Roles.Split(',');
                foreach (var role in roles)
                {
                    if (await _identityService.IsInRoleAsync(role.Trim()))
                    {
                        authorized = true;
                        break;
                    }
                }
                if (!authorized)
                {
                    return Result<TResponse>.Forbidden("User is not in the required role(s).");
                }
            }

            if (!string.IsNullOrWhiteSpace(authorizeAttribute.Policy))
            {
                if (!await _identityService.AuthorizeAsync(authorizeAttribute.Policy))
                {
                    return Result<TResponse>.Forbidden("User does not meet the policy requirements.");
                }
            }
        }

        // Build the closed generic interface type for this request
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, typeof(TResponse));

        var handler = _provider.GetService(handlerType);

        if (handler is null)
        {
            return Result<TResponse>.NotFound($"Handler for request type '{requestType.Name}' not found.");
        }

        // Call ExecuteAsync via reflection
        var method = handlerType.GetMethod("HandleAsync");
        if (method == null)
        {
            return Result<TResponse>.ServerError("Handler does not implement HandleAsync.");
        }

        var task = (Task<Result<TResponse>>)method.Invoke(handler, [request, cancellationToken])!;
        return await task;
    }
}
