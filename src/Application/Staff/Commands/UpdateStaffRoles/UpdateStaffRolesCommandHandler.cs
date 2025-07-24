namespace BaseTemplate.Application.Staff.Commands.UpdateStaffRoles;

public class UpdateStaffRolesCommandHandler : IRequestHandler<UpdateStaffRolesCommand, bool>
{
    private readonly IUnitOfWorkFactory _factory;
    private readonly IUserTenantProfileService _userProfileService;

    public UpdateStaffRolesCommandHandler(IUnitOfWorkFactory factory, IUserTenantProfileService userProfileService)
    {
        _factory = factory;
        _userProfileService = userProfileService;
    }

    public async Task<Result<bool>> HandleAsync(UpdateStaffRolesCommand request, CancellationToken cancellationToken)
    {
        // Get user profile to get tenant ID
        var userProfile = await _userProfileService.GetUserProfileAsync();

        var tenantId = userProfile.TenantId;

        using var uow = _factory.Create();
        using var transaction = uow.BeginTransaction();
        // Verify the user exists and belongs to the tenant
        var user = await uow.QuerySingleAsync<AppUser>(
            "SELECT * FROM app_user WHERE id = @StaffId AND tenant_id = @TenantId",
            new { request.StaffId, TenantId = tenantId });


        // Remove all existing roles for the user (soft delete)
        var existingRoles = await uow.QueryAsync<UserRole>(
            "SELECT * FROM user_role WHERE user_id = @StaffId AND is_deleted = FALSE",
            new { request.StaffId });
        foreach (var role in existingRoles)
        {
            role.IsDeleted = true;
            await uow.UpdateAsync(role);
        }

        // Add new roles
        if (request.NewRoles.Any())
        {
            var roleValues = request.NewRoles.Select(role => new { UserId = request.StaffId, Role = role });
            await uow.ExecuteAsync(
                "INSERT INTO user_role (user_id, role, created, last_modified) VALUES (@UserId, @Role, @Created, @LastModified)",
                roleValues.Select(r => new { r.UserId, r.Role, Created = DateTimeOffset.UtcNow, LastModified = DateTimeOffset.UtcNow }));
        }
        await _userProfileService.InvalidateUserProfileCacheAsync(user.SsoId);
        transaction.Commit();
        return Result<bool>.Success(true);
    }
}
