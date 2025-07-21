namespace BaseTemplate.Application.Tenants.Queries.GetTenantById;

public class GetTenantByIdQueryHandler : IRequestHandler<GetTenantByIdQuery, GetTenantResponse>
{
    private readonly IUnitOfWorkFactory _factory;
    private readonly IUserTenantProfileService _userProfileService;
    public GetTenantByIdQueryHandler(IUnitOfWorkFactory factory, IUserTenantProfileService userProfileService)
    {
        _factory = factory;
        _userProfileService = userProfileService;
    }

    public async Task<Result<GetTenantResponse>> HandleAsync(GetTenantByIdQuery request, CancellationToken cancellationToken)
    {
        var userProfile = await _userProfileService.GetUserProfileAsync();
        using var uow = _factory.Create();
        var entity = await uow.QuerySingleAsync<Tenant>("SELECT * FROM tenant WHERE id = @TenantId AND is_deleted = FALSE", new { TenantId = userProfile.TenantId });
        if (entity is null)
        {
            return Result<GetTenantResponse>.NotFound($"Tenant with id {userProfile.TenantId} not found.");
        }
        var tenant = new GetTenantResponse() { Name = entity.Name, Id = entity.Id, Address = entity.Address };
        return Result<GetTenantResponse>.Success(tenant);
    }
}
