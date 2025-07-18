namespace BaseTemplate.Application.Tenants.Queries.GetTenantById;

public class GetTenantByIdQueryHandler : IRequestHandler<GetTenantByIdQuery, GetTenantResponse>
{
    private readonly IUnitOfWorkFactory _factory;
    private readonly IUserProfileService _userProfileService;
    public GetTenantByIdQueryHandler(IUnitOfWorkFactory factory, IUserProfileService userProfileService)
    {
        _factory = factory;
        _userProfileService = userProfileService;
    }

    public async Task<Result<GetTenantResponse>> HandleAsync(GetTenantByIdQuery request, CancellationToken cancellationToken)
    {
        var userProfile = await _userProfileService.GetUserProfileAsync();
        if (userProfile == null || userProfile.TenantId == null)
        {
            return Result<GetTenantResponse>.NotFound("Current user does not have a tenant.");
        }
        using var uow = _factory.Create();
        var entity = await uow.GetAsync<Tenant>(userProfile.TenantId.Value);
        if (entity is null)
        {
            return Result<GetTenantResponse>.NotFound($"Tenant with id {userProfile.TenantId} not found.");
        }
        var tenant = new GetTenantResponse() { Name = entity.Name, Id = entity.Id, Address = entity.Address };
        return Result<GetTenantResponse>.Success(tenant);
    }
} 