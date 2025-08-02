namespace BaseTemplate.Application.Common.RequestHandler;

public class RequestHandlerResolver(IServiceProvider provider) : IRequestHandlerResolver
{
    private readonly IServiceProvider _provider = provider;

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
