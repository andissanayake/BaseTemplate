namespace BaseTemplate.Application.Staff.Commands.RespondToStaffInvitation;

[Authorize]
public record RespondToStaffInvitationCommand : IRequest<bool>
{
    public int StaffRequestId { get; set; }
    public bool IsAccepted { get; set; }
    public string? RejectionReason { get; set; }
}
