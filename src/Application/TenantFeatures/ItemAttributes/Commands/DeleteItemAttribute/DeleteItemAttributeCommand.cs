namespace BaseTemplate.Application.TenantFeatures.ItemAttributes.Commands.DeleteItemAttribute;

[Authorize(Roles = Roles.AttributeManager)]
public record DeleteItemAttributeCommand(int Id) : IRequest<bool>;
