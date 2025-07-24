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
        var userProfile = await _userProfileService.GetUserProfileAsync();
        var entity = await _context.Tenant.FirstOrDefaultAsync(t => t.Id == userProfile.TenantId && !t.IsDeleted, cancellationToken);
        if (entity is null)
        {
            return Result<GetTenantResponse>.NotFound($"Tenant with id {userProfile.TenantId} not found.");
        }
        var tenant = new GetTenantResponse { Name = entity.Name, Id = entity.Id, Address = entity.Address };
        return Result<GetTenantResponse>.Success(tenant);
    }
}
