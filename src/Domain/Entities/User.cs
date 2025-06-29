namespace BaseTemplate.Domain.Entities;
public class AppUser : BaseAuditableEntity
{
    public string SsoId { get; set; } = string.Empty;
    public string? Name { get; set; }
    public string? Email { get; set; }
    public int? TenantId { get; set; } // User belongs to one tenant
}
