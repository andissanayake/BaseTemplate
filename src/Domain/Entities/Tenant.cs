namespace BaseTemplate.Domain.Entities;
public class Tenant : BaseAuditableEntity
{
    public string UniqName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public int OwnerId { get; set; }
}
