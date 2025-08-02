namespace BaseTemplate.Application.TenantFeatures.ItemAttributes.Commands.UpdateItemAttribute;

[Authorize(Roles = Roles.AttributeManager)]
public class UpdateItemAttributeCommand : IRequest<bool>
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}
