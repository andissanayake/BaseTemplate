namespace BaseTemplate.Application.Common.RequestHandler.Behaviors;

public class HandlerExecutionBehavior : IMediatorBehavior
{
    public async Task<Result> HandleAsync<TResponse>(MediatorContext<TResponse> context)
    {
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(context.RequestType, typeof(TResponse));
        var handler = context.ServiceProvider.GetService(handlerType);

        if (handler is null)
        {
            return Result.NotFound($"Handler for request type '{context.RequestType.Name}' not found.");
        }

        var method = handlerType.GetMethod("HandleAsync");
        if (method == null)
        {
            return Result.ServerError("Handler does not implement HandleAsync.");
        }

        try
        {
            var task = (Task<Result<TResponse>>)method.Invoke(handler, [context.Request, context.CancellationToken])!;
            context.Result = await task;
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.ServerError($"Handler execution failed: {ex.Message}");
        }
    }
} 