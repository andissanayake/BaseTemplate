namespace BaseTemplate.Application.Common.RequestHandler;

public interface IMediatorPipeline
{
    Task<Result<TResponse>> SendAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);
} 