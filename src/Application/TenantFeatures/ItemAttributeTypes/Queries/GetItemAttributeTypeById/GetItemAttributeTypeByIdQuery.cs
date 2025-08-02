namespace BaseTemplate.Application.TenantFeatures.ItemAttributeTypes.Queries.GetItemAttributeTypeById;

[Authorize(Roles = Roles.AttributeManager)]
public class GetItemAttributeTypeByIdQuery : IRequest<ItemAttributeTypeDto>
{
    public int Id { get; set; }
}
