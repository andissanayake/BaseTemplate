namespace BaseTemplate.Application.Tenants.Queries.GetTenantById;

[Authorize]
public record GetTenantByIdQuery : IRequest<GetTenantResponse>; 