using Microsoft.EntityFrameworkCore;
namespace BaseTemplate.Application.Tenants.Queries.GetTenantById;

public class GetTenantByIdQueryHandler : IRequestHandler<GetTenantByIdQuery, GetTenantResponse>
{
    private readonly IAppDbContext _context;
    private readonly IUserProfileService _userProfileService;
    public GetTenantByIdQueryHandler(IAppDbContext context, IUserProfileService userProfileService)
    {
        _context = context;
        _userProfileService = userProfileService;
    }

    public async Task<Result<GetTenantResponse>> HandleAsync(GetTenantByIdQuery request, CancellationToken cancellationToken)
    {
        var userProfile = _userProfileService.UserProfile;
        var entity = await _context.Tenant.SingleAsync(t => t.Id == userProfile.TenantId, cancellationToken);

        var tenant = new GetTenantResponse { Name = entity.Name, Id = entity.Id, Address = entity.Address };
        return Result<GetTenantResponse>.Success(tenant);
    }
}
