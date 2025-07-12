namespace BaseTemplate.Application.Common.RequestHandler;
public class Mediator : IMediator
{
    private readonly IRequestValidator _validator;
    private readonly IRequestAuthorizer _authorizer;
    private readonly IRequestHandlerResolver _handlerResolver;

    public Mediator(
        IServiceProvider provider,
        IRequestValidator validator,
        IRequestAuthorizer authorizer,
        IRequestHandlerResolver handlerResolver)
    {
        _validator = validator;
        _authorizer = authorizer;
        _handlerResolver = handlerResolver;
    }

    public async Task<Result<TResponse>> SendAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        // Validate request is not null
        var nullValidation = _validator.ValidateNull(request);
        if (!ResultCodeMapper.IsSuccess(nullValidation.Code))
        {
            return Result<TResponse>.Validation(nullValidation.Message!, nullValidation.Details);
        }

        var requestType = request.GetType();

        // Authorize request
        var authorizationResult = await _authorizer.AuthorizeAsync(request, requestType);
        if (!ResultCodeMapper.IsSuccess(authorizationResult.Code))
        {
            return Result<TResponse>.Unauthorized(authorizationResult.Message!);
        }

        // Validate request data
        var validationResult = _validator.ValidateRequest(request);
        if (!ResultCodeMapper.IsSuccess(validationResult.Code))
        {
            return Result<TResponse>.Validation(validationResult.Message!, validationResult.Details);
        }

        // Resolve and execute handler
        var handlerResult = await _handlerResolver.ResolveAndExecuteAsync(request, requestType, cancellationToken);

        return handlerResult;
    }
}
