namespace BaseTemplate.Application.TenantFeatures.Items.Queries.GetItemVariantByItemId;

[Authorize(Roles = Roles.ItemManager)]
public record GetItemVariantByItemIdQuery(int ItemId) : IRequest<List<ItemVariantDto>> { }
