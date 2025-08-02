namespace BaseTemplate.Application.TenantFeatures.Items.Commands.DeleteItem;

[Authorize(Roles = Roles.ItemManager)]
public record DeleteItemCommand(int Id) : IRequest<bool>;
