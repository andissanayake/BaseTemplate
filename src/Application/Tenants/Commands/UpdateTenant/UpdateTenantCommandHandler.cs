using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.Application.Tenants.Commands.UpdateTenant;

public class UpdateTenantCommandHandler : IRequestHandler<UpdateTenantCommand, bool>
{
    private readonly IAppDbContext _context;
    private readonly IUserProfileService _userProfileService;
    
    public UpdateTenantCommandHandler(IAppDbContext context, IUserProfileService userProfileService)
    {
        _context = context;
        _userProfileService = userProfileService;
    }
    
    public async Task<Result<bool>> HandleAsync(UpdateTenantCommand request, CancellationToken cancellationToken)
    {
        var userProfile = await _userProfileService.GetUserProfileAsync();
        var entity = await _context.Tenant
            .FirstOrDefaultAsync(t => t.Id == userProfile.TenantId && !t.IsDeleted, cancellationToken);

        if (entity is null)
        {
            return Result<bool>.NotFound($"Tenant with id {userProfile.TenantId} not found.");
        }
        entity.Name = request.Name;
        entity.Address = request.Address;
        await _userProfileService.InvalidateUserProfileCacheAsync();
        await _context.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }
} 