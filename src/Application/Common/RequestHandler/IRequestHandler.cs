namespace BaseTemplate.Application.Common.RequestHandler;

public interface IRequestHandler<in TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    Task<Result<TResponse>> HandleAsync(TRequest request, CancellationToken cancellationToken);
} 