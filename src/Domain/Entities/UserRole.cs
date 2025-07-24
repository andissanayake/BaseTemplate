namespace BaseTemplate.Domain.Entities;
public class UserRole : BaseAuditableEntity
{
    public int UserId { get; set; }
    public AppUser AppUser { get; set; }
    public string Role { get; set; } = string.Empty;
}
