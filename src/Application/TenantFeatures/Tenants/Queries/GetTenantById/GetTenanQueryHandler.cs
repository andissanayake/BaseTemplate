using Microsoft.EntityFrameworkCore;
namespace BaseTemplate.Application.TenantFeatures.Tenants.Queries.GetTenantById;

public class GetTenanQueryHandler(IAppDbContext context, IUserProfileService userProfileService) : IRequestHandler<GetTenantQuery, GetTenantResponse>
{
    private readonly IAppDbContext _context = context;
    private readonly IUserProfileService _userProfileService = userProfileService;

    public async Task<Result<GetTenantResponse>> HandleAsync(GetTenantQuery request, CancellationToken cancellationToken)
    {
        var userProfile = _userProfileService.UserProfile;
        var entity = await _context.Tenant.AsNoTracking().SingleAsync(t => t.Id == userProfile.TenantId, cancellationToken);

        var tenant = new GetTenantResponse { Name = entity.Name, Id = entity.Id, Address = entity.Address };
        return Result<GetTenantResponse>.Success(tenant);
    }
}
