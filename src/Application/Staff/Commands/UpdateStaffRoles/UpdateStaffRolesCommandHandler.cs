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

        // Get all existing roles for the user (including deleted ones)
        var existingRoles = await _context.UserRole
            .Where(r => r.UserId == request.StaffId)
            .ToListAsync(cancellationToken);

        // Mark all existing roles as deleted
        foreach (var role in existingRoles)
        {
            role.IsDeleted = true;
        }

        // Add new roles or reactivate existing ones
        if (request.NewRoles.Any())
        {
            foreach (var newRole in request.NewRoles)
            {
                // Check if this role already exists (including deleted ones)
                var existingRole = existingRoles.FirstOrDefault(r => r.Role == newRole);
                
                if (existingRole != null)
                {
                    // Reactivate the existing role
                    existingRole.IsDeleted = false;
                }
                else
                {
                    // Create a new role entry
                    var userRole = new UserRole
                    {
                        UserId = request.StaffId,
                        Role = newRole
                    };
                    _context.UserRole.Add(userRole);
                }
            }
        }
        
        await _context.SaveChangesAsync(cancellationToken);
        await _userProfileService.InvalidateUserProfileCacheAsync(user.SsoId);
        return Result<bool>.Success(true);
    }
}
