namespace BaseTemplate.Application.Staff.Commands.UpdateStaffRequest;

public class UpdateStaffRequestCommandHandler : IRequestHandler<UpdateStaffRequestCommand, bool>
{
    private readonly IUnitOfWorkFactory _factory;
    private readonly IUserTenantProfileService _userTenantProfileService;

    public UpdateStaffRequestCommandHandler(IUnitOfWorkFactory factory, IUserTenantProfileService userTenantProfileService)
    {
        _factory = factory;
        _userTenantProfileService = userTenantProfileService;
    }

    public async Task<Result<bool>> HandleAsync(UpdateStaffRequestCommand request, CancellationToken cancellationToken)
    {
        // Get user profile to get tenant ID
        var userProfile = await _userTenantProfileService.GetUserProfileAsync();

        using var uow = _factory.Create();

        // Verify the tenant exists and the current user is the owner
        var tenant = await uow.GetAsync<Tenant>(userProfile.TenantId);

        // Get the staff request and verify it belongs to this tenant
        var staffRequest = await uow.QuerySingleAsync<StaffRequest>(
            "SELECT * FROM staff_request WHERE id = @Id AND tenant_id = @TenantId",
            new { Id = request.StaffRequestId, TenantId = userProfile.TenantId });

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
