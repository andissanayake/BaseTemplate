using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.Application.TenantFeatures.Tenants.Commands.UpdateTenant;

public class UpdateTenantCommandHandler(IAppDbContext context, IUserProfileService userProfileService) : IRequestHandler<UpdateTenantCommand, bool>
{
    private readonly IAppDbContext _context = context;
    private readonly IUserProfileService _userProfileService = userProfileService;

    public async Task<Result<bool>> HandleAsync(UpdateTenantCommand request, CancellationToken cancellationToken)
    {
        var userProfile = _userProfileService.UserProfile;

        var entity = await _context.Tenant
            .SingleAsync(t => t.Id == userProfile.TenantId, cancellationToken);

        entity.Name = request.Name;
        entity.Address = request.Address;
        await _context.SaveChangesAsync(cancellationToken);

        await _userProfileService.InvalidateUserProfileCacheAsync();
        return Result<bool>.Success(true);
    }
}
