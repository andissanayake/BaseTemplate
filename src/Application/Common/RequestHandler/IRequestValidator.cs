namespace BaseTemplate.Application.Common.RequestHandler;

public interface IRequestValidator
{
    Result ValidateNull<TResponse>(IRequest<TResponse>? request);
    Result ValidateRequest<TResponse>(IRequest<TResponse> request);
} 