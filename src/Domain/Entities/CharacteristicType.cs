namespace BaseTemplate.Domain.Entities;

public class CharacteristicType : BaseTenantAuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
} 
