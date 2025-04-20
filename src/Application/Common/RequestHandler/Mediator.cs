using System.Reflection;
using BaseTemplate.Application.Common.Interfaces;
using BaseTemplate.Application.Common.Models;
using BaseTemplate.Application.Common.Security;
using BaseTemplate.Application.Common.Validation;

namespace BaseTemplate.Application.Common.RequestHandler;
public interface IRequest<TResponse> { }

public interface IRequestHandler<in TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    Task<Result<TResponse>> AuthorizeAsync(TRequest request, CancellationToken cancellationToken);
    Task<Result<TResponse>> ValidateAsync(TRequest request, CancellationToken cancellationToken);
    Task<Result<TResponse>> HandleAsync(TRequest request, CancellationToken cancellationToken);
    Task<Result<TResponse>> ExecuteAsync(TRequest request, CancellationToken cancellationToken);
}

/*
public interface IRequestHandler<in TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{

    public virtual async Task<Result<TResponse>> AuthorizeAsync(TRequest request, CancellationToken cancellationToken) =>
        Result<TResponse>.Success(default!);

    public virtual async Task<Result<TResponse>> ValidateAsync(TRequest request, CancellationToken cancellationToken)
    {
        var result = ModelValidator.Validate(request);
        if (result.IsValied)
            return Result<TResponse>.Success(default!);
        return Result<TResponse>.Validation("Validation Errors", result.Errors);
    }

    public Task<Result<TResponse>> HandleAsync(TRequest request, CancellationToken cancellationToken);

    public virtual async Task<Result<TResponse>> ExecuteAsync(TRequest request, CancellationToken cancellationToken)
    {
        var authResult = await AuthorizeAsync(request, cancellationToken);
        if (!ResultCodeMapper.IsSuccess(authResult.Code))
            return authResult;

        var validationResult = await ValidateAsync(request, cancellationToken);
        if (!ResultCodeMapper.IsSuccess(validationResult.Code))
            return validationResult;

        return await HandleAsync(request, cancellationToken);
    }
}
*/
public abstract class BaseRequestHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IIdentityService _identityService;

    public BaseRequestHandler(
        IIdentityService identityService)
    {
        _identityService = identityService;
    }
    public virtual Task<Result<TResponse>> ValidateAsync(TRequest request, CancellationToken cancellationToken)
    {
        var result = ModelValidator.Validate(request);
        if (result.IsValied)
            return Task.FromResult(Result<TResponse>.Success(default!));
        return Task.FromResult(Result<TResponse>.Validation("Validation Errors", result.Errors));
    }
    public virtual async Task<Result<TResponse>> AuthorizeAsync(TRequest request, CancellationToken cancellationToken)
    {
        var attributes = request.GetType().GetCustomAttributes<AuthorizeAttribute>();

        if (!attributes.Any()) return Result<TResponse>.Success(default!);

        if (!_identityService.IsAuthenticated)
            return Result<TResponse>.Unauthorized(default!);

        var roles = attributes
            .Where(a => !string.IsNullOrWhiteSpace(a.Roles))
            .SelectMany(a => a.Roles.Split(',').Select(r => r.Trim()));

        if (roles.Any())
        {
            var isAuthorizedByRole = await Task.WhenAny(roles.Select(_identityService.IsInRoleAsync));
            if (!isAuthorizedByRole.Result)
                return Result<TResponse>.Forbidden(default!);
        }

        foreach (var policy in attributes
            .Where(a => !string.IsNullOrWhiteSpace(a.Policy))
            .Select(a => a.Policy))
        {
            if (!await _identityService.AuthorizeAsync(policy))
                return Result<TResponse>.Forbidden(default!);
        }

        return Result<TResponse>.Success(default!);
    }
    public abstract Task<Result<TResponse>> HandleAsync(TRequest request, CancellationToken cancellationToken);

    public async Task<Result<TResponse>> ExecuteAsync(TRequest request, CancellationToken cancellationToken)
    {
        var authResult = await AuthorizeAsync(request, cancellationToken);
        if (!ResultCodeMapper.IsSuccess(authResult.Code))
            return authResult;

        var validationResult = await ValidateAsync(request, cancellationToken);
        if (!ResultCodeMapper.IsSuccess(validationResult.Code))
            return validationResult;

        return await HandleAsync(request, cancellationToken);
    }
}

public interface IMediator
{
    Task<Result<TResponse>> SendAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);
}

public class Mediator : IMediator
{
    private readonly IServiceProvider _provider;

    public Mediator(IServiceProvider provider)
    {
        _provider = provider;
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

        // Build the closed generic interface type for this request
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(request.GetType(), typeof(TResponse));

        var handler = _provider.GetService(handlerType);

        if (handler is null)
        {
            return Result<TResponse>.NotFound($"Handler for request type '{request.GetType().Name}' not found.");
        }

        // Call ExecuteAsync via reflection
        var method = handlerType.GetMethod("ExecuteAsync");
        if (method == null)
        {
            return Result<TResponse>.ServerError("Handler does not implement ExecuteAsync.");
        }

        var task = (Task<Result<TResponse>>)method.Invoke(handler, [request, cancellationToken])!;
        return await task;
    }
}
