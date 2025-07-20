namespace BaseTemplate.Application.ItemAttributes.Queries.GetItemAttributeById;

[Authorize(Roles = Roles.AttributeManager)]
public record GetItemAttributeByIdQuery(int Id) : IRequest<ItemAttributeDto>;
