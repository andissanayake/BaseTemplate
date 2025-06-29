namespace BaseTemplate.Application.Common.RequestHandler;

[Authorize]
public record BaseTenantRequest<TResponse>(int TenantId) : IRequest<TResponse>
{
}
