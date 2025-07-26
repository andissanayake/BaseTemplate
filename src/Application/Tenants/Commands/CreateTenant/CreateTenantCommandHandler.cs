using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.Application.Tenants.Commands.CreateTenant;

public class CreateTenantCommandHandler : IRequestHandler<CreateTenantCommand, int>
{
    private readonly IAppDbContext _context;
    private readonly IUser _user;
    private readonly IUserProfileService _userProfileService;

    public CreateTenantCommandHandler(IAppDbContext context, IUser user, IUserProfileService userProfileService)
    {
        _context = context;
        _user = user;
        _userProfileService = userProfileService;
    }

    public async Task<Result<int>> HandleAsync(CreateTenantCommand request, CancellationToken cancellationToken)
    {
        // Load existing user - user should already exist at this point
        var existingUser = await _context.AppUser
            .SingleAsync(u => u.SsoId == _user.Identifier, cancellationToken);

        // Create new tenant
        var tenant = new Tenant
        {
            Name = request.Name,
            Address = request.Address,
            OwnerSsoId = _user.Identifier
        };
        _context.Tenant.Add(tenant);
        await _context.SaveChangesAsync(cancellationToken);

        // Update user's tenant_id
        existingUser.TenantId = tenant.Id;
        existingUser.LastModified = DateTimeOffset.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        // Add TenantOwner role to the user
        var userRole = new UserRole
        {
            UserId = existingUser.Id,
            Role = Roles.TenantOwner
        };
        _context.UserRole.Add(userRole);
        await _userProfileService.InvalidateUserProfileCacheAsync();
        await _context.SaveChangesAsync(cancellationToken);
        return Result<int>.Success(tenant.Id);
    }
}
