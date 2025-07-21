namespace BaseTemplate.Application.ItemAttributes.Commands.CreateItemAttribute;

[Authorize(Roles = Roles.AttributeManager)]
public class CreateItemAttributeCommand : IRequest<int>
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public int ItemAttributeTypeId { get; set; }
}
