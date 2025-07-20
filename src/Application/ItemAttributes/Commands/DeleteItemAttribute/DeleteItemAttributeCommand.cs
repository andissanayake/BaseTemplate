using BaseTemplate.Domain.Constants;

namespace BaseTemplate.Application.ItemAttributes.Commands.DeleteItemAttribute;
 
[Authorize(Roles = Roles.AttributeManager)]
public record DeleteItemAttributeCommand(int Id) : IRequest<bool>; 