using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.Application.TenantFeatures.Staff.Commands.UpdateStaffRoles;

public class UpdateStaffRolesCommandHandler(IAppDbContext context, IUserProfileService userProfileService) : IRequestHandler<UpdateStaffRolesCommand, bool>
{
    private readonly IAppDbContext _context = context;
    private readonly IUserProfileService _userProfileService = userProfileService;

    public async Task<Result<bool>> HandleAsync(UpdateStaffRolesCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.AppUser
            .SingleAsync(u => u.Id == request.StaffId, cancellationToken);

        var existingRoles = await _context.UserRole
            .Where(r => r.UserId == request.StaffId)
            .ToListAsync(cancellationToken);

        // Mark all existing roles as deleted
        foreach (var role in existingRoles)
        {
            role.IsDeleted = true;
            _context.UserRole.Update(role);
        }
        if (request.NewRoles.Count != 0)
        {
            foreach (var newRole in request.NewRoles)
            {
                var existingRole = existingRoles.FirstOrDefault(r => r.Role == newRole);
                if (existingRole != null)
                {
                    existingRole.IsDeleted = false;
                    _context.UserRole.Update(existingRole);
                }
                else
                {
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
