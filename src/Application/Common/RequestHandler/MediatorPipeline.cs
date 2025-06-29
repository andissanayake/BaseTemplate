namespace BaseTemplate.Application.Common.RequestHandler;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using BaseTemplate.Application.Common.Interfaces;
using BaseTemplate.Application.Common.Security;
using Microsoft.Extensions.DependencyInjection;

// Pipeline-based mediator for even simpler architecture
public interface IMediatorPipeline
{
    Task<Result<TResponse>> SendAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);
}

public class MediatorPipeline : IMediatorPipeline
{
    private readonly IServiceProvider _provider;
    private readonly IEnumerable<IMediatorBehavior> _behaviors;

    public MediatorPipeline(IServiceProvider provider, IEnumerable<IMediatorBehavior> behaviors)
    {
        _provider = provider;
        _behaviors = behaviors;
    }

    public async Task<Result<TResponse>> SendAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        var context = new MediatorContext<TResponse>(request, _provider, cancellationToken);

        // Execute behaviors in order
        foreach (var behavior in _behaviors)
        {
            var result = await behavior.HandleAsync(context);
            if (!ResultCodeMapper.IsSuccess(result.Code))
            {
                return Result<TResponse>.Failure(result.Message!, result.Code);
            }
        }

        return context.Result!;
    }
}

// Behavior interface for pipeline steps
public interface IMediatorBehavior
{
    Task<Result> HandleAsync<TResponse>(MediatorContext<TResponse> context);
}

// Context to pass data through the pipeline
public class MediatorContext<TResponse>
{
    public IRequest<TResponse> Request { get; }
    public IServiceProvider ServiceProvider { get; }
    public CancellationToken CancellationToken { get; }
    public Result<TResponse>? Result { get; set; }
    public Type RequestType { get; }

    public MediatorContext(IRequest<TResponse> request, IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        Request = request;
        ServiceProvider = serviceProvider;
        CancellationToken = cancellationToken;
        RequestType = request.GetType();
    }
}

// Individual behaviors
public class NullValidationBehavior : IMediatorBehavior
{
    public Task<Result> HandleAsync<TResponse>(MediatorContext<TResponse> context)
    {
        if (context.Request is null)
        {
            return Task.FromResult(Result.Validation("Request is required", []));
        }
        return Task.FromResult(Result.Success());
    }
}

public class RequestValidationBehavior : IMediatorBehavior
{
    public Task<Result> HandleAsync<TResponse>(MediatorContext<TResponse> context)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(context.Request, null, null);

        if (!Validator.TryValidateObject(context.Request, validationContext, validationResults, true))
        {
            var errors = validationResults
                .SelectMany(vr => vr.MemberNames.Select(mn => new { MemberName = mn, vr.ErrorMessage }))
                .GroupBy(e => e.MemberName)
                .ToDictionary(g => g.Key.ToLower(), g => g.Select(e => e.ErrorMessage ?? "Validation error").ToArray());

            return Task.FromResult(Result.Validation("Request validation failed", errors));
        }

        return Task.FromResult(Result.Success());
    }
}

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

public class HandlerExecutionBehavior : IMediatorBehavior
{
    public async Task<Result> HandleAsync<TResponse>(MediatorContext<TResponse> context)
    {
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(context.RequestType, typeof(TResponse));
        var handler = context.ServiceProvider.GetService(handlerType);

        if (handler is null)
        {
            return Result.NotFound($"Handler for request type '{context.RequestType.Name}' not found.");
        }

        var method = handlerType.GetMethod("HandleAsync");
        if (method == null)
        {
            return Result.ServerError("Handler does not implement HandleAsync.");
        }

        try
        {
            var task = (Task<Result<TResponse>>)method.Invoke(handler, [context.Request, context.CancellationToken])!;
            context.Result = await task;
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.ServerError($"Handler execution failed: {ex.Message}");
        }
    }
}
