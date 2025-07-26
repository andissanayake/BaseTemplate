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
            .SingleAsync(t => t.Id == userProfile.TenantId && !t.IsDeleted, cancellationToken);

        entity.Name = request.Name;
        entity.Address = request.Address;
        await _context.SaveChangesAsync(cancellationToken);
        await _userProfileService.InvalidateUserProfileCacheAsync();

        return Result<bool>.Success(true);
    }
}
