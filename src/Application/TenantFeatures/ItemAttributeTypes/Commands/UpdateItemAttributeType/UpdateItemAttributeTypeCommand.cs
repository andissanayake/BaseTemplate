namespace BaseTemplate.Application.TenantFeatures.ItemAttributeTypes.Commands.UpdateItemAttributeType;

[Authorize(Roles = Roles.AttributeManager)]
public class UpdateItemAttributeTypeCommand : IRequest<bool>
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
