namespace BaseTemplate.Application.Common.RequestHandler;

public class MediatorPipeline : IMediatorPipeline
{
    private readonly IServiceProvider _provider;
    private readonly IEnumerable<IMediatorBehavior> _behaviors;

    public MediatorPipeline(IServiceProvider provider, IEnumerable<IMediatorBehavior> behaviors)
    {
        _provider = provider;
        _behaviors = behaviors;
    }

    public async Task<Result<TResponse>> SendAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        var context = new MediatorContext<TResponse>(request, _provider, cancellationToken);

        // Execute behaviors in order
        foreach (var behavior in _behaviors)
        {
            var result = await behavior.HandleAsync(context);
            if (!ResultCodeMapper.IsSuccess(result.Code))
            {
                return Result<TResponse>.Failure(result.Message!, result.Code);
            }
        }

        return context.Result!;
    }
}
