namespace BaseTemplate.Application.TenantFeatures.Tenants.Queries.GetTenantById;

[Authorize(Roles = Roles.TenantManager)]
public record GetTenantQuery : IRequest<GetTenantResponse>;
