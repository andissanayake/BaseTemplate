namespace BaseTemplate.Application.TenantFeatures.Staff.Commands.RevokeStaffInvitation;

[Authorize(Roles = Roles.StaffInvitationManager)]
public record RevokeStaffInvitationCommand : IRequest<bool>
{
    public int StaffInvitationId { get; set; }
    public required string RejectionReason { get; set; }
}
