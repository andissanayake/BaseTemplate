namespace BaseTemplate.Domain.Entities;

public class Item : BaseTenantAuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public bool IsActive { get; set; } = true;
    public string? Category { get; set; }
}
