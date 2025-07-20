using BaseTemplate.Domain.Constants;

namespace BaseTemplate.Application.ItemAttributeTypes.Commands.CreateItemAttributeType;

[Authorize(Roles = Roles.AttributeManager)]
public class CreateItemAttributeTypeCommand : IRequest<int>
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
} 