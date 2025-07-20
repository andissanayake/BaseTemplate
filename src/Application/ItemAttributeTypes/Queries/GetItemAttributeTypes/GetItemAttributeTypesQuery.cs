using BaseTemplate.Domain.Constants;

namespace BaseTemplate.Application.ItemAttributeTypes.Queries.GetItemAttributeTypes;

[Authorize(Roles = Roles.AttributeManager)]
public class GetItemAttributeTypesQuery : IRequest<List<ItemAttributeTypeBriefDto>>
{
} 