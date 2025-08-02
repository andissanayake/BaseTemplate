namespace BaseTemplate.Application.Staff.Queries.GetStaffInvitation;

public class StaffInvitationDto
{
    public int Id { get; set; }
    public string RequestedEmail { get; set; } = string.Empty;
    public string RequestedName { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
    public int RequestedByAppUserId { get; set; }
    public string RequestedByAppUserName { get; set; } = string.Empty;
    public string RequestedByAppUserEmail { get; set; } = string.Empty;
    public StaffInvitationStatus Status { get; set; }
    public DateTimeOffset Created { get; set; }
    public DateTimeOffset? AcceptedAt { get; set; }
    public int? AcceptedByAppUserId { get; set; }
    public string? AcceptedByAppUserName { get; set; }
    public string? AcceptedByAppUserEmail { get; set; }
    public string? RejectionReason { get; set; }
} 