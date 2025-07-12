namespace BaseTemplate.Application.Common.RequestHandler.Behaviors;

public class NullValidationBehavior : IMediatorBehavior
{
    public Task<Result> HandleAsync<TResponse>(MediatorContext<TResponse> context)
    {
        if (context.Request is null)
        {
            return Task.FromResult(Result.Validation("Request is required", []));
        }
        return Task.FromResult(Result.Success());
    }
} 