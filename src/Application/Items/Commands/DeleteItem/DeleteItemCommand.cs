using BaseTemplate.Domain.Constants;

namespace BaseTemplate.Application.Items.Commands.DeleteItem;

[Authorize(Roles = Roles.ItemManager + "," + Roles.TenantOwner)]
public record DeleteItemCommand(int Id) : IRequest<bool>; 