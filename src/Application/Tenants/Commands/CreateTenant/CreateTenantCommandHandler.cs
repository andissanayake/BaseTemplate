using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.Application.Tenants.Commands.CreateTenant;

public class CreateTenantCommandHandler : IRequestHandler<CreateTenantCommand, int>
{
    private readonly IAppDbContext _context;
    private readonly IUser _user;
    private readonly IUserProfileService _userTenantProfileService;

    public CreateTenantCommandHandler(IAppDbContext context, IUser user, IUserProfileService userTenantProfileService)
    {
        _context = context;
        _user = user;
        _userTenantProfileService = userTenantProfileService;
    }

    public async Task<Result<int>> HandleAsync(CreateTenantCommand request, CancellationToken cancellationToken)
    {
        // Load existing user - user should already exist at this point
        var existingUser = await _context.AppUser
            .FirstOrDefaultAsync(u => u.SsoId == _user.Identifier, cancellationToken);
        if (existingUser == null)
        {
            return Result<int>.NotFound($"User with sso_id {_user.Identifier} not found.");
        }

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
        await _userTenantProfileService.InvalidateUserProfileCacheAsync();
        await _context.SaveChangesAsync(cancellationToken);
        return Result<int>.Success(tenant.Id);
    }
} 