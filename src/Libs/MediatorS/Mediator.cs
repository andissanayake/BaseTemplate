namespace MediatorS
{
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

            var requestType = request.GetType();
            var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, typeof(TResponse));

            var handler = _provider.GetService(handlerType);
            if (handler is null)
            {
                return Result<TResponse>.NotFound($"Handler for request type '{requestType.Name}' not found.");
            }

            var methodInfo = handlerType.GetMethod("HandleAsync");
            if (methodInfo is null)
            {
                return Result<TResponse>.ServerError($"HandleAsync method not found on handler '{handlerType.Name}'.");
            }

            var task = (Task<Result<TResponse>>)methodInfo.Invoke(handler, new object[] { request, cancellationToken })!;
            return await task;
        }
    }
}
