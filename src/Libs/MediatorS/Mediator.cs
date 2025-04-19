using Microsoft.Extensions.DependencyInjection;

namespace MediatorS;
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

