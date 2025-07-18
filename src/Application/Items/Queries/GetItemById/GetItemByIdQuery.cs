namespace BaseTemplate.Application.Items.Queries.GetItemById;

[Authorize]
public record GetItemByIdQuery(int Id) : IRequest<ItemDto> { }
