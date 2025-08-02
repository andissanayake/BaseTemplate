namespace BaseTemplate.Application.TenantFeatures.Items.Queries.GetItemById;

[Authorize(Roles = Roles.ItemManager)]
public record GetItemByIdQuery(int Id) : IRequest<ItemDto> { }
