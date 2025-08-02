namespace BaseTemplate.Application.TenantFeatures.Tenants.Queries.GetTenantById;

[Authorize(Roles = Roles.TenantManager)]
public record GetTenantByIdQuery : IRequest<GetTenantResponse>;
