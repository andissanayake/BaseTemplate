namespace BaseTemplate.Application.Tenants.Commands.CreateTenant;

public class CreateTenantCommandHandler : IRequestHandler<CreateTenantCommand, int>
{
    private readonly IUnitOfWorkFactory _factory;
    private readonly IUser _user;
    private readonly IUserTenantProfileService _userTenantProfileService;

    public CreateTenantCommandHandler(IUnitOfWorkFactory factory, IUser user, IUserTenantProfileService userTenantProfileService)
    {
        _factory = factory;
        _user = user;
        _userTenantProfileService = userTenantProfileService;
    }

    public async Task<Result<int>> HandleAsync(CreateTenantCommand request, CancellationToken cancellationToken)
    {
        using var uow = _factory.Create();
        using var transaction = uow.BeginTransaction();

        // Load existing user - user should already exist at this point
        var existingUser = await uow.QuerySingleAsync<AppUser>(
            "SELECT * FROM app_user WHERE sso_id = @SsoId",
            new { SsoId = _user.Identifier });

        // Create new tenant
        var tenant = new Tenant
        {
            Name = request.Name,
            Address = request.Address,
            OwnerSsoId = _user.Identifier
        };
        var tenantId = await uow.InsertAsync(tenant);

        // Update user's tenant_id
        await uow.ExecuteAsync(
            "UPDATE app_user SET tenant_id = @TenantId, last_modified = @LastModified WHERE id = @UserId",
            new { TenantId = tenantId, LastModified = DateTimeOffset.UtcNow, UserId = existingUser.Id });

        // Add TenantOwner role to the user
        var userRole = new UserRole
        {
            UserId = existingUser.Id,
            Role = Roles.TenantOwner
        };
        await _userTenantProfileService.InvalidateUserProfileCacheAsync();
        await uow.InsertAsync(userRole);
        transaction.Commit();
        return Result<int>.Success(tenantId);
    }
} 