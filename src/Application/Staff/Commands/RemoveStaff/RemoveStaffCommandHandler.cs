using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.Application.Staff.Commands.RemoveStaff;

public class RemoveStaffCommandHandler : IRequestHandler<RemoveStaffCommand, bool>
{
    private readonly IAppDbContext _context;
    private readonly IUserProfileService _userProfileService;

    public RemoveStaffCommandHandler(IAppDbContext context, IUserProfileService userProfileService)
    {
        _context = context;
        _userProfileService = userProfileService;
    }

    public async Task<Result<bool>> HandleAsync(RemoveStaffCommand request, CancellationToken cancellationToken)
    {
        // Get user profile to get tenant ID
        var userProfile = await _userProfileService.GetUserProfileAsync();

        // Verify the user exists and belongs to the tenant
        var user = await _context.AppUser
            .FirstOrDefaultAsync(u => u.Id == request.StaffId && u.TenantId == userProfile.TenantId && !u.IsDeleted, cancellationToken);
        if (user == null)
        {
            return Result<bool>.NotFound($"User with id {request.StaffId} not found in tenant.");
        }

        // Soft delete all roles for the user
        var roles = await _context.UserRole
            .Where(r => r.UserId == request.StaffId && !r.IsDeleted)
            .ToListAsync(cancellationToken);
        foreach (var role in roles)
        {
            role.IsDeleted = true;
        }

        // Soft delete the user (set is_deleted = true)
        user.IsDeleted = true;

        // Set related staff requests to Expired
        var staffRequests = await _context.StaffRequest
            .Where(sr => sr.RequestedEmail == user.Email && sr.TenantId == userProfile.TenantId && sr.Status == StaffRequestStatus.Accepted)
            .ToListAsync(cancellationToken);
        foreach (var sr in staffRequests)
        {
            sr.Status = StaffRequestStatus.Expired;
        }
        await _userProfileService.InvalidateUserProfileCacheAsync(user.SsoId);
        await _context.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }
}
