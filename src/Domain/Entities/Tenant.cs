namespace BaseTemplate.Domain.Entities;
public class Tenant : BaseAuditableEntity
{
    public string Identifier { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string OwnerIdentifier { get; set; } = string.Empty;
}
