namespace BaseTemplate.Domain.Entities;
public class ItemCharacteristicType : BaseTenantAuditableEntity
{
    public int ItemId { get; set; }
    public Item Item { get; set; } = default!;
    public int CharacteristicTypeId { get; set; }
    public CharacteristicType CharacteristicType { get; set; } = default!;
}
