namespace BaseTemplate.Domain.Entities;

public class ItemAttribute : BaseTenantAuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public int ItemAttributeTypeId { get; set; }
    public ItemAttributeType? ItemAttributeType { get; set; }

}
