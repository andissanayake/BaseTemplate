namespace BaseTemplate.Application.Users.Queries.GetUser;

public record GetUserResponse
{
    public IEnumerable<string> Roles { get; set; } = new List<string>();
    public TenantDetails? Tenant { get; set; }
    public StaffRequestDetails? StaffRequest { get; set; }
} 