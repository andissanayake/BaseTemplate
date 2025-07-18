namespace BaseTemplate.Application.Users.Queries;

public record TenantDetails
{
    public required int Id { get; set; }
    public required string Name { get; set; }
}
