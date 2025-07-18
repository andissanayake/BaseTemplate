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

        // Verify the user exists and belongs to the tenant
        var user = await uow.QuerySingleAsync<AppUser>(
            "SELECT * FROM app_user WHERE id = @StaffId AND tenant_id = @TenantId",
            new { request.StaffId, TenantId = userProfile.TenantId });

        // Remove all roles for the user
        await uow.ExecuteAsync(
            "DELETE FROM user_role WHERE user_id = @StaffId",
            new { request.StaffId });

        // Remove the user from the tenant (set tenant_id to null)
        await uow.ExecuteAsync(
            "UPDATE app_user SET tenant_id = NULL, last_modified = @LastModified WHERE id = @StaffId",
            new { request.StaffId, LastModified = DateTimeOffset.UtcNow });

        // Set related staff requests to Expired
        await uow.ExecuteAsync(
            "UPDATE staff_request SET status = @ExpiredStatus WHERE requested_email = @Email AND tenant_id = @TenantId AND status = @AcceptedStatus",
            new
            {
                ExpiredStatus = (int)StaffRequestStatus.Expired,
                AcceptedStatus = (int)StaffRequestStatus.Accepted,
                Email = user.Email,
                TenantId = userProfile.TenantId
            });

        return Result<bool>.Success(true);
    }
}
