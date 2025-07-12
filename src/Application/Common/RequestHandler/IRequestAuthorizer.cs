namespace BaseTemplate.Application.Common.RequestHandler;

public interface IRequestAuthorizer
{
    Task<Result> AuthorizeAsync<TResponse>(IRequest<TResponse> request, Type requestType);
} 