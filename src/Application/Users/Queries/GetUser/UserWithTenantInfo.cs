namespace BaseTemplate.Application.Users.Queries.GetUser;

public record UserWithTenantInfo
{
    public required int Id { get; set; }
    public required string SsoId { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public int? TenantId { get; set; }
    public string? TenantName { get; set; }
}
