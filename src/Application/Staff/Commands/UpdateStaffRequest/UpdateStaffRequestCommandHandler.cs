using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.Application.Staff.Commands.UpdateStaffRequest;

public class UpdateStaffRequestCommandHandler : IRequestHandler<UpdateStaffRequestCommand, bool>
{
    private readonly IAppDbContext _context;
    private readonly IUserProfileService _userTenantProfileService;

    public UpdateStaffRequestCommandHandler(IAppDbContext context, IUserProfileService userTenantProfileService)
    {
        _context = context;
        _userTenantProfileService = userTenantProfileService;
    }

    public async Task<Result<bool>> HandleAsync(UpdateStaffRequestCommand request, CancellationToken cancellationToken)
    {
        // Get user profile to get tenant ID
        var userProfile = await _userTenantProfileService.GetUserProfileAsync();

        // Get the staff request and verify it belongs to this tenant
        var staffRequest = await _context.StaffRequest
            .FirstOrDefaultAsync(sr => sr.Id == request.StaffRequestId && sr.TenantId == userProfile.TenantId, cancellationToken);

        if (staffRequest == null)
        {
            return Result<bool>.NotFound($"Staff request with id {request.StaffRequestId} not found.");
        }

        if (staffRequest.Status != StaffRequestStatus.Pending)
        {
            return Result<bool>.Validation("This staff request has already been processed.");
        }

        // Reject the request
        staffRequest.Status = StaffRequestStatus.Revoked;
        staffRequest.RejectionReason = request.RejectionReason;
        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true, $"Staff request for {staffRequest.RequestedEmail} has been rejected.");
    }
}
