namespace BaseTemplate.Domain.Entities;
public class AppUser : BaseAuditableEntity
{
    public string Identifier { get; set; } = string.Empty;
    public string? Name { get; set; }
    public string? Email { get; set; }
}
