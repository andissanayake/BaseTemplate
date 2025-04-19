

using MediatorS;
using Microsoft.AspNetCore.Mvc;

namespace BaseTemplate.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class ApiControllerBase : ControllerBase
{
    private IMediator _mediator = null!;

    protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<IMediator>();

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
                var code when code == ResultCodeMapper.DefaultValidationErrorCode => BadRequest(result.Details),
                var code when code == ResultCodeMapper.DefaultServerErrorCode => StatusCode(500, result.Details),
                var code when code == ResultCodeMapper.DefaultNotFoundCode => NotFound(result.Details),
                var code when code == ResultCodeMapper.DefaultUnauthorizedCode => Unauthorized(result.Details),
                _ => BadRequest(result.Details)
            };
        }
        catch (Exception ex)
        {
            var errorDetails = new Dictionary<string, string[]>
            {
                ["error"] = new[] { ex.Message }
            };
            return StatusCode(500, errorDetails);
        }
    }
}
