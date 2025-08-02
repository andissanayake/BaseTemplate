namespace BaseTemplate.Application.GlobalFeatures.Staff.Commands.RespondToStaffInvitation;

[Authorize]
public record RespondToStaffInvitationCommand : IRequest<bool>
{
    public int StaffInvitationId { get; set; }
    public bool IsAccepted { get; set; }
    public string? RejectionReason { get; set; }
}
