using BaseTemplate.Domain.Constants;

namespace BaseTemplate.Application.Items.Commands.DeleteItem;

[Authorize(Roles = Roles.TenantOwner)]
public record DeleteItemCommand(int TenantId, int Id) : BaseTenantRequest<bool>(TenantId); 