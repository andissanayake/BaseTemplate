namespace BaseTemplate.Application.Common.RequestHandler;

public interface IRequestHandlerResolver
{
    Task<Result<TResponse>> ResolveAndExecuteAsync<TResponse>(IRequest<TResponse> request, Type requestType, CancellationToken cancellationToken);
} 