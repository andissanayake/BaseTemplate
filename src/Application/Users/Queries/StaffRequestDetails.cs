namespace BaseTemplate.Application.Users.Queries;

public record StaffRequestDetails
{
    public int Id { get; set; }
    public string RequesterName { get; set; } = string.Empty;
    public string RequesterEmail { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
    public StaffRequestStatus Status { get; set; }
    public DateTimeOffset Created { get; set; }
    public string TenantName { get; set; } = string.Empty;
} 