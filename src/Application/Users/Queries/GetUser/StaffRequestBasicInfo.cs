namespace BaseTemplate.Application.Users.Queries.GetUser;

public record StaffRequestBasicInfo
{
    public int Id { get; set; }
    public string RequestedBySsoId { get; set; } = string.Empty;
    public StaffRequestStatus Status { get; set; }
    public DateTimeOffset Created { get; set; }
    public string TenantName { get; set; } = string.Empty;
} 