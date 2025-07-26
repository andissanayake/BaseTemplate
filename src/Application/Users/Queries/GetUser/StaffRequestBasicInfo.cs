namespace BaseTemplate.Application.Users.Queries.GetUser;

public record StaffRequestBasicInfo
{
    public int Id { get; set; }
    public int RequestedByAppUserId { get; set; }
    public StaffRequestStatus Status { get; set; }
    public DateTimeOffset Created { get; set; }
    public string TenantName { get; set; } = string.Empty;
} 