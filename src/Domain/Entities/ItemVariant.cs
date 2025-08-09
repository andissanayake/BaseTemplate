namespace BaseTemplate.Domain.Entities;

public class ItemVariant : BaseTenantAuditableEntity
{
    public int ItemId { get; set; }
    public Item Item { get; set; } = default!;
    public string Code { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public List<ItemVariantCharacteristic> ItemVariantCharacteristicList { get; set; } = [];
}
