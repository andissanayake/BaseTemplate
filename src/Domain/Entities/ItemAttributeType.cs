namespace BaseTemplate.Domain.Entities;

public class ItemAttributeType : BaseTenantAuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
} 