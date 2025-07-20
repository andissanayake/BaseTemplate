using BaseTemplate.Domain.Constants;

namespace BaseTemplate.Application.ItemAttributeTypes.Queries.GetItemAttributeTypeById;

[Authorize(Roles = Roles.AttributeManager)]
public class GetItemAttributeTypeByIdQuery : IRequest<ItemAttributeTypeDto>
{
    public int Id { get; set; }
} 