namespace BaseTemplate.Application.Staff.Commands.UpdateStaffRequest;

public class UpdateStaffRequestCommandHandler : IRequestHandler<UpdateStaffRequestCommand, bool>
{
    private readonly IUnitOfWorkFactory _factory;
    private readonly IUserProfileService _userProfileService;

    public UpdateStaffRequestCommandHandler(IUnitOfWorkFactory factory, IUserProfileService userProfileService)
    {
        _factory = factory;
        _userProfileService = userProfileService;
    }

    public async Task<Result<bool>> HandleAsync(UpdateStaffRequestCommand request, CancellationToken cancellationToken)
    {
        // Get user profile to get tenant ID
        var userProfile = await _userProfileService.GetUserProfileAsync();
        if (userProfile?.TenantId == null)
        {
            return Result<bool>.Forbidden("User is not associated with any tenant.");
        }

        var tenantId = userProfile.TenantId.Value;

        using var uow = _factory.Create();

        // Verify the tenant exists and the current user is the owner
        var tenant = await uow.GetAsync<Tenant>(tenantId);
        if (tenant == null)
        {
            return Result<bool>.NotFound($"Tenant with id {tenantId} not found.");
        }

        // Get the staff request and verify it belongs to this tenant
        var staffRequest = await uow.QueryFirstOrDefaultAsync<StaffRequest>(
            "SELECT * FROM staff_request WHERE id = @Id AND tenant_id = @TenantId",
            new { Id = request.StaffRequestId, TenantId = tenantId });

        if (staffRequest == null)
        {
            return Result<bool>.NotFound($"Staff request with id {request.StaffRequestId} not found for this tenant.");
        }

        if (staffRequest.Status != StaffRequestStatus.Pending)
        {
            return Result<bool>.Validation("This staff request has already been processed.");
        }

        // Reject the request
        staffRequest.Status = StaffRequestStatus.Revoked;
        staffRequest.RejectionReason = request.RejectionReason;
        await uow.UpdateAsync(staffRequest);

        return Result<bool>.Success(true, $"Staff request for {staffRequest.RequestedEmail} has been rejected.");

    }
}
