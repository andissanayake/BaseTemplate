using Microsoft.EntityFrameworkCore;
namespace BaseTemplate.Application.TenantFeatures.Staff.Commands.RevokeStaffInvitation;

public class RevokeStaffInvitationCommandHandler(IAppDbContext context) : IRequestHandler<RevokeStaffInvitationCommand, bool>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<bool>> HandleAsync(RevokeStaffInvitationCommand request, CancellationToken cancellationToken)
    {
        var staffRequest = await _context.StaffInvitation
            .SingleAsync(sr => sr.Id == request.StaffRequestId && sr.Status == StaffInvitationStatus.Pending, cancellationToken);

        staffRequest.Status = StaffInvitationStatus.Revoked;
        staffRequest.RejectionReason = request.RejectionReason;
        await _context.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true, $"Staff request for {staffRequest.RequestedEmail} has been rejected.");
    }
}
