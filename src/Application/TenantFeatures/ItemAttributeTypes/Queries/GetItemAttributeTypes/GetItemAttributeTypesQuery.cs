namespace BaseTemplate.Application.TenantFeatures.ItemAttributeTypes.Queries.GetItemAttributeTypes;

[Authorize(Roles = Roles.AttributeManager)]
public class GetItemAttributeTypesQuery : IRequest<List<ItemAttributeTypeBriefDto>>
{
}
