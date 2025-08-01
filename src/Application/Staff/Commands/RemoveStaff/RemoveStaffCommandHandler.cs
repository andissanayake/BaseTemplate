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
        var userProfile = await _userProfileService.GetUserProfileAsync();

        var user = await _context.AppUser
            .SingleAsync(u => u.Id == request.StaffId && u.TenantId == userProfile.TenantId && !u.IsDeleted, cancellationToken);
        user.TenantId = null;
        _context.AppUser.Update(user);

        var roles = await _context.UserRole
            .Where(r => r.UserId == request.StaffId && !r.IsDeleted)
            .ToListAsync(cancellationToken);
        foreach (var role in roles)
        {
            role.IsDeleted = true;
        }

        var staffRequest = await _context.StaffInvitation
            .Where(sr => sr.RequestedEmail == user.Email && sr.TenantId == userProfile.TenantId && sr.Status == StaffRequestStatus.Accepted)
            .SingleAsync(cancellationToken);
        staffRequest.Status = StaffRequestStatus.Expired;

        await _context.SaveChangesAsync(cancellationToken);

        await _userProfileService.InvalidateUserProfileCacheAsync(user.SsoId);
        return Result<bool>.Success(true);
    }
}
