namespace BaseTemplate.Application.Staff.Queries.GetStaffRequests;

public class StaffRequestDto
{
    public int Id { get; set; }
    public string RequestedEmail { get; set; } = string.Empty;
    public string RequestedName { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
    public string RequestedBySsoId { get; set; } = string.Empty;
    public StaffRequestStatus Status { get; set; }
    public DateTimeOffset Created { get; set; }
    public DateTimeOffset? AcceptedAt { get; set; }
    public string? AcceptedBySsoId { get; set; }
    public string? RejectionReason { get; set; }
} 