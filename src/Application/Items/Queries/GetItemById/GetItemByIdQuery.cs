namespace BaseTemplate.Application.Items.Queries.GetItemById;

[Authorize]
public record GetItemByIdQuery(int TenantId, int Id) : BaseTenantRequest<ItemDto>(TenantId); 