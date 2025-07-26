using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.Application.Staff.Commands.UpdateStaffRoles;

public class UpdateStaffRolesCommandHandler : IRequestHandler<UpdateStaffRolesCommand, bool>
{
    private readonly IAppDbContext _context;
    private readonly IUserProfileService _userProfileService;

    public UpdateStaffRolesCommandHandler(IAppDbContext context, IUserProfileService userProfileService)
    {
        _context = context;
        _userProfileService = userProfileService;
    }

    public async Task<Result<bool>> HandleAsync(UpdateStaffRolesCommand request, CancellationToken cancellationToken)
    {
        var userProfile = await _userProfileService.GetUserProfileAsync();

        var user = await _context.AppUser
            .SingleAsync(u => u.Id == request.StaffId && u.TenantId == userProfile.TenantId, cancellationToken);

        var existingRoles = await _context.UserRole
            .Where(r => r.UserId == request.StaffId && !r.IsDeleted)
            .ToListAsync(cancellationToken);

        foreach (var role in existingRoles)
        {
            role.IsDeleted = true;
        }

        // Add new roles
        if (request.NewRoles.Any())
        {
            foreach (var role in request.NewRoles)
            {
                var userRole = new UserRole
                {
                    UserId = request.StaffId,
                    Role = role
                };
                _context.UserRole.Add(userRole);
            }
        }
        await _context.SaveChangesAsync(cancellationToken);
        await _userProfileService.InvalidateUserProfileCacheAsync(user.SsoId);
        return Result<bool>.Success(true);
    }
}
