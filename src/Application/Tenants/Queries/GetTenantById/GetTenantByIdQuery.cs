namespace BaseTemplate.Application.Tenants.Queries.GetTenantById;

[Authorize]
public record GetTenantByIdQuery(int TenantId) : BaseTenantRequest<GetTenantResponse>(TenantId); 