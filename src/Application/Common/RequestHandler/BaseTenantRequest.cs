using BaseTemplate.Domain.Constants;
using BaseTemplate.Application.Common.Security;

namespace BaseTemplate.Application.Common.RequestHandler;

[Authorize]
public record BaseTenantRequest<TResponse> : IRequest<TResponse>
{
    public int TenantId { get; set; }
}
