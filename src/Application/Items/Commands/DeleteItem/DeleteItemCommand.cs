namespace BaseTemplate.Application.Items.Commands.DeleteItem;

[Authorize(Roles = Roles.ItemManager)]
public record DeleteItemCommand(int Id) : IRequest<bool>;
