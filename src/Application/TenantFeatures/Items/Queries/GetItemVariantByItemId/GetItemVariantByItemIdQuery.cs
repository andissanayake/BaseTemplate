namespace BaseTemplate.Application.TenantFeatures.Items.Queries.GetItemVariantByItemId;

[Authorize(Roles = Roles.ItemManager)]
public record GetItemVariantsByItemIdQuery(int ItemId) : IRequest<List<ItemVariantDto>> { }
