namespace BaseTemplate.Application.Common.RequestHandler;

public interface IMediatorBehavior
{
    Task<Result> HandleAsync<TResponse>(MediatorContext<TResponse> context);
} 