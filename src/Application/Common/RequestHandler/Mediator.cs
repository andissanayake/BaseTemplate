using BaseTemplate.Application.Common.Models;

namespace BaseTemplate.Application.Common.RequestHandler;
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
        var method = handlerType.GetMethod("HandleAsync");
        if (method == null)
        {
            return Result<TResponse>.ServerError("Handler does not implement HandleAsync.");
        }

        var task = (Task<Result<TResponse>>)method.Invoke(handler, [request, cancellationToken])!;
        return await task;
    }
}
