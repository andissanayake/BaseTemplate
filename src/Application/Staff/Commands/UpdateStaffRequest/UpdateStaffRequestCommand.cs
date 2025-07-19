namespace BaseTemplate.Application.Staff.Commands.UpdateStaffRequest;

[Authorize(Roles = Roles.StaffRequestManager)]
public record UpdateStaffRequestCommand : IRequest<bool>
{
    public int StaffRequestId { get; set; }
    public required string RejectionReason { get; set; }
} 