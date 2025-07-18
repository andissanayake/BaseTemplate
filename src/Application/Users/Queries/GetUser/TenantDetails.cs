namespace BaseTemplate.Application.Users.Queries.GetUser;

public record TenantDetails
{
    public required int Id { get; set; }
    public required string Name { get; set; }
}
