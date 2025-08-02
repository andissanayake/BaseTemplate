namespace BaseTemplate.Application.TenantFeatures.Staff.Queries.ListStaff;

public class StaffMemberDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public List<string> Roles { get; set; } = new();
    public DateTimeOffset Created { get; set; }
    public DateTimeOffset? LastModified { get; set; }
} 