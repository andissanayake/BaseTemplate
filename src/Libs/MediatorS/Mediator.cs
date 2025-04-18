namespace MediatorS
{
    public interface IRequest<TResponse> { }

    public interface IRequestHandler<in TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        public virtual async Task<Result<TResponse>> ExecuteAsync(TRequest request, CancellationToken cancellationToken)
        {
            var authResult = await AuthorizeAsync(request, cancellationToken);
            if (!ResultCodeMapper.IsSuccess(authResult.Code))
                return authResult;

            var validationResult = await ValidateAsync(request, cancellationToken);
            if (!ResultCodeMapper.IsSuccess(validationResult.Code))
                return validationResult;

            return await HandleAsync(request, cancellationToken);
        }

        public virtual Task<Result<TResponse>> AuthorizeAsync(TRequest request, CancellationToken cancellationToken) =>
            Task.FromResult(Result<TResponse>.Success(default!));

        public virtual Task<Result<TResponse>> ValidateAsync(TRequest request, CancellationToken cancellationToken) =>
            Task.FromResult(Result<TResponse>.Success(default!));

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

            var methodInfo = handlerType.GetMethod("ExecuteAsync");
            if (methodInfo is null)
            {
                return Result<TResponse>.ServerError($"ExecuteAsync method not found on handler '{handlerType.Name}'.");
            }

            var task = (Task<Result<TResponse>>)methodInfo.Invoke(handler, new object[] { request, cancellationToken })!;
            return await task;
        }
    }
    /*
    public abstract class RequestHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        public virtual async Task<Result<TResponse>> ExecuteAsync(TRequest request, CancellationToken cancellationToken)
        {
            var authResult = await AuthorizeAsync(request, cancellationToken);
            if (!ResultCodeMapper.IsSuccess(authResult.Code))
                return authResult;

            var validationResult = await ValidateAsync(request, cancellationToken);
            if (!ResultCodeMapper.IsSuccess(validationResult.Code))
                return validationResult;

            return await HandleAsync(request, cancellationToken);
        }

        public virtual Task<Result<TResponse>> AuthorizeAsync(TRequest request, CancellationToken cancellationToken) =>
            Task.FromResult(Result<TResponse>.Success(default!));

        public virtual Task<Result<TResponse>> ValidateAsync(TRequest request, CancellationToken cancellationToken) =>
            Task.FromResult(Result<TResponse>.Success(default!));

        public abstract Task<Result<TResponse>> HandleAsync(TRequest request, CancellationToken cancellationToken);
    }
    */
}
