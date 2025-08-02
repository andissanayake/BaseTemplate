namespace BaseTemplate.Application.TenantFeatures.ItemAttributes.Queries.GetItemAttributeById;

[Authorize(Roles = Roles.AttributeManager)]
public record GetItemAttributeByIdQuery(int Id) : IRequest<ItemAttributeDto>;
