using Microsoft.EntityFrameworkCore;
namespace BaseTemplate.Application.Staff.Commands.RevokeStaffInvitation;

public class RevokeStaffInvitationCommandHandler : IRequestHandler<RevokeStaffInvitationCommand, bool>
{
    private readonly IAppDbContext _context;

    public RevokeStaffInvitationCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> HandleAsync(RevokeStaffInvitationCommand request, CancellationToken cancellationToken)
    {
        var staffRequest = await _context.StaffInvitation
            .SingleAsync(sr => sr.Id == request.StaffRequestId && sr.Status == StaffRequestStatus.Pending, cancellationToken);

        // Reject the request
        staffRequest.Status = StaffRequestStatus.Revoked;
        staffRequest.RejectionReason = request.RejectionReason;
        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true, $"Staff request for {staffRequest.RequestedEmail} has been rejected.");
    }
}
