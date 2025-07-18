namespace BaseTemplate.Application.Items.Queries.GetItemById;

[Authorize(Roles = Roles.ItemManager)]
public record GetItemByIdQuery(int Id) : IRequest<ItemDto> { }
