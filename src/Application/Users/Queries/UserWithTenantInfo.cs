namespace BaseTemplate.Application.Users.Queries;

public record UserWithTenantInfo
{
    public int Id { get; set; }
    public string SsoId { get; set; } = string.Empty;
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? TenantId { get; set; }
    public string? TenantName { get; set; }
} 