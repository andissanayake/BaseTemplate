namespace BaseTemplate.Domain.Entities;

public class StaffInvitation : BaseTenantAuditableEntity
{
    public string RequestedEmail { get; set; } = string.Empty;
    public string RequestedName { get; set; } = string.Empty;
    public int RequestedByAppUserId { get; set; }
    public AppUser RequestedByAppUser { get; set; } = null!;
    public StaffRequestStatus Status { get; set; } = StaffRequestStatus.Pending;
    public DateTimeOffset? AcceptedAt { get; set; }
    public int? AcceptedByAppUserId { get; set; }
    public AppUser? AcceptedByAppUser { get; set; }
    public string? RejectionReason { get; set; }
}

public class StaffInvitationRole : BaseTenantAuditableEntity
{
    public int StaffInvitationId { get; set; }
    public string Role { get; set; } = string.Empty;
}

public enum StaffRequestStatus
{
    Pending,
    Accepted,
    Rejected,
    Revoked,
    Expired
}
