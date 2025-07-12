namespace BaseTemplate.Application.Tenants.Queries.GetTenantById;

public record GetTenantResponse
{
    public int Id { get; init; }
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
} 