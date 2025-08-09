namespace BaseTemplate.Domain.Entities;

public class Item : BaseTenantAuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public string? Tags { get; set; }
    public int SpecificationId { get; set; }
    public bool HasVariant { get; set; }
    public Specification Specification { get; set; } = default!;
    public List<ItemCharacteristicType> ItemCharacteristicTypeList { get; set; } = [];
}
