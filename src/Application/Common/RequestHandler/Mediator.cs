namespace BaseTemplate.Application.Common.RequestHandler;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using BaseTemplate.Application.Common.Interfaces;
using BaseTemplate.Application.Common.Security;

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
    private readonly IRequestValidator _validator;
    private readonly IRequestAuthorizer _authorizer;
    private readonly IRequestHandlerResolver _handlerResolver;

    public Mediator(
        IServiceProvider provider,
        IRequestValidator validator,
        IRequestAuthorizer authorizer,
        IRequestHandlerResolver handlerResolver)
    {
        _validator = validator;
        _authorizer = authorizer;
        _handlerResolver = handlerResolver;
    }

    public async Task<Result<TResponse>> SendAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        // Validate request is not null
        var nullValidation = _validator.ValidateNull(request);
        if (!ResultCodeMapper.IsSuccess(nullValidation.Code))
        {
            return Result<TResponse>.Validation(nullValidation.Message!, nullValidation.Details);
        }

        var requestType = request.GetType();

        // Authorize request
        var authorizationResult = await _authorizer.AuthorizeAsync(request, requestType);
        if (!ResultCodeMapper.IsSuccess(authorizationResult.Code))
        {
            return Result<TResponse>.Unauthorized(authorizationResult.Message!);
        }

        // Validate request data
        var validationResult = _validator.ValidateRequest(request);
        if (!ResultCodeMapper.IsSuccess(validationResult.Code))
        {
            return Result<TResponse>.Validation(validationResult.Message!, validationResult.Details);
        }

        // Resolve and execute handler
        var handlerResult = await _handlerResolver.ResolveAndExecuteAsync<TResponse>(request, requestType, cancellationToken);
        if (!ResultCodeMapper.IsSuccess(handlerResult.Code))
        {
            return Result<TResponse>.NotFound(handlerResult.Message!);
        }

        return handlerResult;
    }
}

// Separate interfaces for each concern
public interface IRequestValidator
{
    Result ValidateNull<TResponse>(IRequest<TResponse>? request);
    Result ValidateRequest<TResponse>(IRequest<TResponse> request);
}

public interface IRequestAuthorizer
{
    Task<Result> AuthorizeAsync<TResponse>(IRequest<TResponse> request, Type requestType);
}

public interface IRequestHandlerResolver
{
    Task<Result<TResponse>> ResolveAndExecuteAsync<TResponse>(IRequest<TResponse> request, Type requestType, CancellationToken cancellationToken);
}

// Implementation classes
public class RequestValidator : IRequestValidator
{
    public Result ValidateNull<TResponse>(IRequest<TResponse>? request)
    {
        if (request is null)
        {
            return Result.Validation("Request is required", new Dictionary<string, string[]>
            {
                ["request"] = ["Request cannot be null."]
            });
        }
        return Result.Success();
    }

    public Result ValidateRequest<TResponse>(IRequest<TResponse> request)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(request, null, null);

        if (!Validator.TryValidateObject(request, validationContext, validationResults, true))
        {
            var errors = validationResults
                .SelectMany(vr => vr.MemberNames.Select(mn => new { MemberName = mn, vr.ErrorMessage }))
                .GroupBy(e => e.MemberName)
                .ToDictionary(g => g.Key.ToLower(), g => g.Select(e => e.ErrorMessage ?? "Validation error").ToArray());

            return Result.Validation("Request validation failed", errors);
        }

        return Result.Success();
    }
}

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

        // Check policy
        if (!string.IsNullOrWhiteSpace(authorizeAttribute.Policy))
        {
            if (!await _identityService.AuthorizeAsync(authorizeAttribute.Policy))
            {
                return Result.Forbidden("User does not meet the policy requirements.");
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

public class RequestHandlerResolver : IRequestHandlerResolver
{
    private readonly IServiceProvider _provider;

    public RequestHandlerResolver(IServiceProvider provider)
    {
        _provider = provider;
    }

    public async Task<Result<TResponse>> ResolveAndExecuteAsync<TResponse>(IRequest<TResponse> request, Type requestType, CancellationToken cancellationToken)
    {
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, typeof(TResponse));
        var handler = _provider.GetService(handlerType);

        if (handler is null)
        {
            return Result<TResponse>.NotFound($"Handler for request type '{requestType.Name}' not found.");
        }

        var method = handlerType.GetMethod("HandleAsync");
        if (method == null)
        {
            return Result<TResponse>.ServerError("Handler does not implement HandleAsync.");
        }

        try
        {
            var task = (Task<Result<TResponse>>)method.Invoke(handler, [request, cancellationToken])!;
            return await task;
        }
        catch (Exception ex)
        {
            return Result<TResponse>.ServerError($"Handler execution failed: {ex.Message}");
        }
    }
}
