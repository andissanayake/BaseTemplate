namespace BaseTemplate.Application.TenantFeatures.ItemAttributes.Queries.GetItemAttributes;

[Authorize(Roles = Roles.AttributeManager)]
public class GetItemAttributesQuery : IRequest<List<ItemAttributeBriefDto>>
{
    public int ItemAttributeTypeId { get; set; }
}
