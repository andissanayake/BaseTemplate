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
        _context.AppUser.Update(existingUser);

        await _context.SaveChangesAsync(cancellationToken);
        
        await _userProfileService.InvalidateUserProfileCacheAsync();
        return Result<int>.Success(tenant.Id);
    }
}
