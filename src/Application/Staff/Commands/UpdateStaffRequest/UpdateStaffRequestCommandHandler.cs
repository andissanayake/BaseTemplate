using Microsoft.EntityFrameworkCore;
namespace BaseTemplate.Application.Staff.Commands.UpdateStaffRequest;

public class UpdateStaffRequestCommandHandler : IRequestHandler<UpdateStaffRequestCommand, bool>
{
    private readonly IAppDbContext _context;
    private readonly IUserProfileService _userProfileService;

    public UpdateStaffRequestCommandHandler(IAppDbContext context, IUserProfileService userProfileService)
    {
        _context = context;
        _userProfileService = userProfileService;
    }

    public async Task<Result<bool>> HandleAsync(UpdateStaffRequestCommand request, CancellationToken cancellationToken)
    {
        var userProfile = await _userProfileService.GetUserProfileAsync();

        var staffRequest = await _context.StaffRequest
            .SingleAsync(sr => sr.Id == request.StaffRequestId && sr.TenantId == userProfile.TenantId && sr.Status == StaffRequestStatus.Pending, cancellationToken);

        // Reject the request
        staffRequest.Status = StaffRequestStatus.Revoked;
        staffRequest.RejectionReason = request.RejectionReason;
        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true, $"Staff request for {staffRequest.RequestedEmail} has been rejected.");
    }
}
