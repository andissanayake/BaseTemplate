namespace BaseTemplate.Application.Tenants.Commands.UpdateStaffRequest;

[Authorize(Roles = Domain.Constants.Roles.TenantOwner)]
public record UpdateStaffRequestCommand : BaseTenantRequest<bool>
{
    public int StaffRequestId { get; set; }
    public bool Accept { get; set; }
    public string? RejectionReason { get; set; }
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
            new { Id = request.StaffRequestId, TenantId = request.TenantId });

        if (staffRequest == null)
        {
            return Result<bool>.NotFound($"Staff request with id {request.StaffRequestId} not found for this tenant.");
        }

        if (staffRequest.Status != StaffRequestStatus.Pending)
        {
            return Result<bool>.Validation("This staff request has already been processed.");
        }

        if (request.Accept)
        {
            // Check if user already exists
            var existingUser = await uow.QueryFirstOrDefaultAsync<AppUser>(
                "SELECT * FROM app_user WHERE email = @Email",
                new { Email = staffRequest.RequestedEmail });

            if (existingUser != null)
            {
                return Result<bool>.Validation($"User with email {staffRequest.RequestedEmail} already exists in the system.");
            }

            // Create new user
            var newUser = new AppUser
            {
                SsoId = Guid.NewGuid().ToString(),
                Name = staffRequest.RequestedName,
                Email = staffRequest.RequestedEmail,
                TenantId = staffRequest.TenantId
            };
            await uow.InsertAsync(newUser);

            // Get roles for this staff request and add them to the user
            var roles = await uow.QueryAsync<StaffRequestRole>(
                "SELECT * FROM staff_request_role WHERE staff_request_id = @StaffRequestId",
                new { StaffRequestId = staffRequest.Id });

            foreach (var role in roles)
            {
                var userRole = new UserRole
                {
                    UserSsoId = newUser.SsoId,
                    Role = role.Role
                };
                await uow.InsertAsync(userRole);
            }

            // Update staff request status
            staffRequest.Status = StaffRequestStatus.Accepted;
            staffRequest.AcceptedAt = DateTimeOffset.UtcNow;
            staffRequest.AcceptedBySsoId = _user.Identifier;
            await uow.UpdateAsync(staffRequest);

            return Result<bool>.Success(true, $"Staff request for {staffRequest.RequestedEmail} has been accepted successfully.");
        }
        else
        {
            // Reject the request
            staffRequest.Status = StaffRequestStatus.Rejected;
            staffRequest.RejectionReason = request.RejectionReason;
            await uow.UpdateAsync(staffRequest);

            return Result<bool>.Success(true, $"Staff request for {staffRequest.RequestedEmail} has been rejected.");
        }
    }
}
