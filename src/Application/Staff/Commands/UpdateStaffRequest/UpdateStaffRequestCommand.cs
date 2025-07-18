namespace BaseTemplate.Application.Staff.Commands.UpdateStaffRequest;

[Authorize(Roles = Domain.Constants.Roles.StaffRequestManager + "," + Domain.Constants.Roles.TenantOwner)]
public record UpdateStaffRequestCommand : IRequest<bool>
{
    public int StaffRequestId { get; set; }
    public required string RejectionReason { get; set; }
} 