using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.Application.TenantFeatures.Tenants.Commands.CreateTenant;

public class CreateTenantCommandHandler(IAppDbContext context, IUser user, IUserProfileService userProfileService) : IRequestHandler<CreateTenantCommand, int>
{
    private readonly IAppDbContext _context = context;
    private readonly IUser _user = user;
    private readonly IUserProfileService _userProfileService = userProfileService;

    public async Task<Result<int>> HandleAsync(CreateTenantCommand request, CancellationToken cancellationToken)
    {
        // Load existing user - user should already exist at this point
        var existingUser = await _context.AppUser
            .SingleAsync(u => u.SsoId == _user.Identifier, cancellationToken);

        var tenant = new Tenant
        {
            Name = request.Name,
            Address = request.Address,
            OwnerId = existingUser.Id
        };
        _context.Tenant.Add(tenant);

        // Add TenantOwner role to the user
        var userRole = new UserRole
        {
            UserId = existingUser.Id,
            Role = Roles.TenantOwner
        };
        _context.UserRole.Add(userRole);
        await _context.SaveChangesAsync(cancellationToken);

        existingUser.TenantId = tenant.Id;
        await _context.SaveChangesAsync(cancellationToken);

        await _userProfileService.InvalidateUserProfileCacheAsync();
        return Result<int>.Success(tenant.Id);
    }
}
