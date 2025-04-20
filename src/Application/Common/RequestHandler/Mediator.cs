using System.Reflection;
using BaseTemplate.Application.Common.Interfaces;
using BaseTemplate.Application.Common.Models;
using BaseTemplate.Application.Common.Security;
using BaseTemplate.Application.Common.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace BaseTemplate.Application.Common.RequestHandler;
public interface IRequest<TResponse> { }

public interface IRequestHandler<in TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
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

    public virtual Task<Result<TResponse>> AuthorizeAsync(TRequest request, CancellationToken cancellationToken) =>
        Task.FromResult(Result<TResponse>.Success(default!));

    public virtual Task<Result<TResponse>> ValidateAsync(TRequest request, CancellationToken cancellationToken)
    {
        var result = ModelValidator.Validate(request);
        if (result.IsValied)
            return Task.FromResult(Result<TResponse>.Success(default!));
        return Task.FromResult(Result<TResponse>.Validation("Validation Errors", result.Errors));
    }

    Task<Result<TResponse>> HandleAsync(TRequest request, CancellationToken cancellationToken);
}

public class BaseRequestHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
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
        var authorizeAttributes = request.GetType().GetCustomAttributes<AuthorizeAttribute>();

        if (authorizeAttributes.Any())
        {
            // Must be authenticated user
            if (_identityService.IsAuthenticated)
            {
                return Result<TResponse>.Unauthorized(default!);
            }
            var authorizeAttributesWithRoles = authorizeAttributes.Where(a => !string.IsNullOrWhiteSpace(a.Roles));

            if (authorizeAttributesWithRoles.Any())
            {
                var authorized = false;

                foreach (var roles in authorizeAttributesWithRoles.Select(a => a.Roles.Split(',')))
                {
                    foreach (var role in roles)
                    {
                        var isInRole = await _identityService.IsInRoleAsync(role.Trim());
                        if (isInRole)
                        {
                            authorized = true;
                            break;
                        }
                    }
                }

                // Must be a member of at least one role in roles
                if (!authorized)
                {
                    return Result<TResponse>.Forbidden(default!);
                }
            }

            // Policy-based authorization
            var authorizeAttributesWithPolicies = authorizeAttributes.Where(a => !string.IsNullOrWhiteSpace(a.Policy));
            if (authorizeAttributesWithPolicies.Any())
            {
                foreach (var policy in authorizeAttributesWithPolicies.Select(a => a.Policy))
                {
                    var authorized = await _identityService.AuthorizeAsync(policy);

                    if (!authorized)
                    {
                        return Result<TResponse>.Forbidden(default!);
                    }
                }
            }
        }
        return Result<TResponse>.Success(default!);
    }
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
    public virtual Task<Result<TResponse>> HandleAsync(TRequest request, CancellationToken cancellationToken) =>
        Task.FromResult(Result<TResponse>.Success(default!));
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

        var handler = _provider.GetService<IRequestHandler<IRequest<TResponse>, TResponse>>();
        if (handler is null)
        {
            return Result<TResponse>.NotFound($"Handler for request type '{request.GetType().Name}' not found.");
        }

        return await handler.ExecuteAsync(request, cancellationToken); ;
    }
}
