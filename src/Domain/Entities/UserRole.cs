namespace BaseTemplate.Domain.Entities;
public class UserRole : BaseAuditableEntity
{
    public string UserSsoId { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}
