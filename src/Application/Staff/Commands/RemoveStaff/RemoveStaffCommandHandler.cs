namespace BaseTemplate.Application.Staff.Commands.RemoveStaff;

public class RemoveStaffCommandHandler : IRequestHandler<RemoveStaffCommand, bool>
{
    private readonly IUnitOfWorkFactory _factory;
    private readonly IUserProfileService _userProfileService;

    public RemoveStaffCommandHandler(IUnitOfWorkFactory factory, IUserProfileService userProfileService)
    {
        _factory = factory;
        _userProfileService = userProfileService;
    }

    public async Task<Result<bool>> HandleAsync(RemoveStaffCommand request, CancellationToken cancellationToken)
    {
        // Get user profile to get tenant ID
        var userProfile = await _userProfileService.GetUserProfileAsync();
        if (userProfile?.TenantId == null)
        {
            return Result<bool>.Forbidden("User is not associated with any tenant.");
        }

        var tenantId = userProfile.TenantId.Value;

        using var uow = _factory.Create();

        // Verify the user exists and belongs to the tenant
        var user = await uow.QueryFirstOrDefaultAsync<AppUser>(
            "SELECT * FROM app_user WHERE id = @StaffId AND tenant_id = @TenantId",
            new { request.StaffId, TenantId = tenantId });

        if (user == null)
        {
            return Result<bool>.Failure("Staff member not found or does not belong to this tenant.");
        }

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
            new { 
                ExpiredStatus = (int)BaseTemplate.Domain.Entities.StaffRequestStatus.Expired,
                AcceptedStatus = (int)BaseTemplate.Domain.Entities.StaffRequestStatus.Accepted,
                Email = user.Email,
                TenantId = tenantId
            });

        return Result<bool>.Success(true);
    }
} 