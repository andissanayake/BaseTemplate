namespace BaseTemplate.Domain.Entities;
public class UserRole : BaseAuditableEntity
{
    public string UserId { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}
