namespace BaseTemplate.Application.Staff.Commands.RemoveStaff;

public class RemoveStaffCommandHandler : IRequestHandler<RemoveStaffCommand, bool>
{
    private readonly IUnitOfWorkFactory _factory;
    private readonly IUserTenantProfileService _userProfileService;

    public RemoveStaffCommandHandler(IUnitOfWorkFactory factory, IUserTenantProfileService userProfileService)
    {
        _factory = factory;
        _userProfileService = userProfileService;
    }

    public async Task<Result<bool>> HandleAsync(RemoveStaffCommand request, CancellationToken cancellationToken)
    {
        // Get user profile to get tenant ID
        var userProfile = await _userProfileService.GetUserProfileAsync();

        using var uow = _factory.Create();
        using var transaction = uow.BeginTransaction();
        // Verify the user exists and belongs to the tenant
        var user = await uow.QuerySingleAsync<AppUser>(
            "SELECT * FROM app_user WHERE id = @StaffId AND tenant_id = @TenantId AND is_deleted = FALSE",
            new { request.StaffId, TenantId = userProfile.TenantId });

        // Soft delete all roles for the user
        var roles = await uow.QueryAsync<UserRole>(
            "SELECT * FROM user_role WHERE user_id = @StaffId AND is_deleted = FALSE",
            new { request.StaffId });
        foreach (var role in roles)
        {
            role.IsDeleted = true;
            await uow.UpdateAsync(role);
        }

        // Soft delete the user (set is_deleted = true)
        user.IsDeleted = true;
        await uow.UpdateAsync(user);

        // Set related staff requests to Expired (no soft delete for staff_request here, but could be added if needed)
        await uow.ExecuteAsync(
            "UPDATE staff_request SET status = @ExpiredStatus WHERE requested_email = @Email AND tenant_id = @TenantId AND status = @AcceptedStatus",
            new
            {
                ExpiredStatus = (int)StaffRequestStatus.Expired,
                AcceptedStatus = (int)StaffRequestStatus.Accepted,
                Email = user.Email,
                TenantId = userProfile.TenantId
            });
        await _userProfileService.InvalidateUserProfileCacheAsync(user.SsoId);
        transaction.Commit();
        return Result<bool>.Success(true);
    }
}
