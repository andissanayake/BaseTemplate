namespace BaseTemplate.Application.Staff.Commands.UpdateStaffRequest;

[Authorize(Roles = Domain.Constants.Roles.StaffRequestManager + "," + Domain.Constants.Roles.TenantOwner)]
public record UpdateStaffRequestCommand(int TenantId) : BaseTenantRequest<bool>(TenantId)
{
    public int StaffRequestId { get; set; }
    public required string RejectionReason { get; set; }
}

public class UpdateStaffRequestCommandHandler : IRequestHandler<UpdateStaffRequestCommand, bool>
{
    private readonly IUnitOfWorkFactory _factory;
    private readonly IUser _user;

    public UpdateStaffRequestCommandHandler(IUnitOfWorkFactory factory, IUser user)
    {
        _factory = factory;
        _user = user;
    }

    public async Task<Result<bool>> HandleAsync(UpdateStaffRequestCommand request, CancellationToken cancellationToken)
    {
        using var uow = _factory.Create();

        // Verify the tenant exists and the current user is the owner
        var tenant = await uow.GetAsync<Tenant>(request.TenantId);
        if (tenant == null)
        {
            return Result<bool>.NotFound($"Tenant with id {request.TenantId} not found.");
        }

        if (tenant.OwnerSsoId != _user.Identifier)
        {
            return Result<bool>.Forbidden("Only tenant owners can update staff requests.");
        }

        // Get the staff request and verify it belongs to this tenant
        var staffRequest = await uow.QueryFirstOrDefaultAsync<StaffRequest>(
            "SELECT * FROM staff_request WHERE id = @Id AND tenant_id = @TenantId",
            new { Id = request.StaffRequestId, request.TenantId });

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
