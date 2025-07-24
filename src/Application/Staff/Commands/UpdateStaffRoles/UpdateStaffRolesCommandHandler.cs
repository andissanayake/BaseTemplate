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
        // Get user profile to get tenant ID
        var userProfile = await _userProfileService.GetUserProfileAsync();
        var tenantId = userProfile.TenantId;

        // Verify the user exists and belongs to the tenant
        var user = await _context.AppUser
            .FirstOrDefaultAsync(u => u.Id == request.StaffId && u.TenantId == tenantId, cancellationToken);
        if (user == null)
        {
            return Result<bool>.NotFound($"User with id {request.StaffId} not found in tenant.");
        }

        // Remove all existing roles for the user (soft delete)
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
        await _userProfileService.InvalidateUserProfileCacheAsync(user.SsoId);
        await _context.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }
}
