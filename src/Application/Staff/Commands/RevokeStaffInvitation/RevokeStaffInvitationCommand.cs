namespace BaseTemplate.Application.Staff.Commands.RevokeStaffInvitation;

[Authorize(Roles = Roles.StaffInvitationManager)]
public record RevokeStaffInvitationCommand : IRequest<bool>
{
    public int StaffRequestId { get; set; }
    public required string RejectionReason { get; set; }
}
