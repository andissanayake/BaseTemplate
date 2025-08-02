using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.Application.TenantFeatures.Staff.Commands.UpdateStaffRoles;

public class UpdateStaffRolesCommandHandler : IRequestHandler<UpdateStaffRolesCommand, bool>
{
    private readonly IAppDbContext _context;

    public UpdateStaffRolesCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<bool>> HandleAsync(UpdateStaffRolesCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.AppUser
            .SingleAsync(u => u.Id == request.StaffId, cancellationToken);

        // Get all existing roles for the user (including deleted ones)
        var existingRoles = await _context.UserRole
            .Where(r => r.UserId == request.StaffId)
            .ToListAsync(cancellationToken);

        // Mark all existing roles as deleted
        foreach (var role in existingRoles)
        {
            role.IsDeleted = true;
            _context.UserRole.Update(role);
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
                    _context.UserRole.Update(existingRole);
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
        return Result<bool>.Success(true);
    }
}
