using BaseTemplate.Domain.Constants;

namespace BaseTemplate.Application.ItemAttributes.Queries.GetItemAttributes;

[Authorize(Roles = Roles.AttributeManager)]
public class GetItemAttributesQuery : IRequest<List<ItemAttributeBriefDto>>
{
    public string? SearchTerm { get; set; }
    public bool? IsActive { get; set; }
    public int? ItemAttributeTypeId { get; set; }
} 