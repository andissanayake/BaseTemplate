using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.Application.TenantFeatures.Staff.Commands.RemoveStaff;

public class RemoveStaffCommandHandler(IAppDbContext context, IUserProfileService userProfileService) : IRequestHandler<RemoveStaffCommand, bool>
{
    private readonly IAppDbContext _context = context;
    private readonly IUserProfileService _userProfileService = userProfileService;

    public async Task<Result<bool>> HandleAsync(RemoveStaffCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.AppUser
            .SingleAsync(u => u.Id == request.StaffId, cancellationToken);
        user.TenantId = null;

        var roles = await _context.UserRole
            .Where(r => r.UserId == request.StaffId)
            .ToListAsync(cancellationToken);
        foreach (var role in roles)
        {
            role.IsDeleted = true;
        }

        var staffRequest = await _context.StaffInvitation
            .Where(sr => sr.RequestedEmail == user.Email && sr.Status == StaffInvitationStatus.Accepted)
            .SingleAsync(cancellationToken);
        staffRequest.Status = StaffInvitationStatus.Expired;
        await _context.SaveChangesAsync(cancellationToken);
        await _userProfileService.InvalidateUserProfileCacheAsync(user.SsoId);
        return Result<bool>.Success(true);
    }
}
