namespace BaseTemplate.Domain.Entities;
public class ItemVariantCharacteristic : BaseTenantAuditableEntity
{
    public int ItemVariantId { get; set; }
    public ItemVariant ItemVariant { get; set; } = default!;
    public int CharacteristicId { get; set; }
    public Characteristic Characteristic { get; set; } = default!;
}
