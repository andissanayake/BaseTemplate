
using BaseTemplate.Application.Common.Models;
using BaseTemplate.Application.Common.RequestHandler;
using Microsoft.AspNetCore.Mvc;

namespace BaseTemplate.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class ApiControllerBase : ControllerBase
{
    private IMediator _mediator = null!;
    private ILogger<ApiControllerBase> _log = null!;

    protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<IMediator>();
    protected ILogger<ApiControllerBase> Log => _log ??= HttpContext.RequestServices.GetRequiredService<ILogger<ApiControllerBase>>();
    public async Task<ActionResult<Result<T>>> SendAsync<T>(
        IRequest<T> request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await Mediator.SendAsync(request, cancellationToken);
            return result.Code switch
            {
                var code when ResultCodeMapper.IsSuccess(code) => Ok(result),
                var code when code == ResultCodeMapper.DefaultValidationErrorCode => BadRequest(result),
                var code when code == ResultCodeMapper.DefaultServerErrorCode => StatusCode(500, result),
                var code when code == ResultCodeMapper.DefaultNotFoundCode => NotFound(result),
                var code when code == ResultCodeMapper.DefaultUnauthorizedCode => Unauthorized(result),
                var code when code == ResultCodeMapper.DefaultForbiddenCode => Forbid(result.Details.SelectMany(kv => kv.Value).ToArray()),
                _ => BadRequest(result)
            };
        }
        catch (Exception ex)
        {
            Log.LogError(ex, "Unhandled Exception", [request]);
            return StatusCode(500, Result<T>.ServerError(ex.Message));
        }
    }
}
