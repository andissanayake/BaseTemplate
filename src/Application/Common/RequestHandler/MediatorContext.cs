namespace BaseTemplate.Application.Common.RequestHandler;

public class MediatorContext<TResponse>
{
    public IRequest<TResponse> Request { get; }
    public IServiceProvider ServiceProvider { get; }
    public CancellationToken CancellationToken { get; }
    public Result<TResponse>? Result { get; set; }
    public Type RequestType { get; }

    public MediatorContext(IRequest<TResponse> request, IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        Request = request;
        ServiceProvider = serviceProvider;
        CancellationToken = cancellationToken;
        RequestType = request.GetType();
    }
} 