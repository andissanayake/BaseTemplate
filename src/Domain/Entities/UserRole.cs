namespace BaseTemplate.Domain.Entities;
public class UserRole : BaseAuditableEntity
{
    public int UserId { get; set; }
    public string Role { get; set; } = string.Empty;
}
