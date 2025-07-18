namespace BaseTemplate.Application.Staff.Commands.RequestStaff;

public class RequestStaffCommandHandler : IRequestHandler<RequestStaffCommand, bool>
{
    private readonly IUnitOfWorkFactory _factory;
    private readonly IUserProfileService _userProfileService;

    public RequestStaffCommandHandler(IUnitOfWorkFactory factory, IUserProfileService userProfileService)
    {
        _factory = factory;
        _userProfileService = userProfileService;
    }

    public async Task<Result<bool>> HandleAsync(RequestStaffCommand request, CancellationToken cancellationToken)
    {
        var userProfile = await _userProfileService.GetUserProfileAsync();
        var tenantId = await _userProfileService.GetTenantIdAsync();
        using var uow = _factory.Create();


        // Validate that only allowed roles can be requested
        var invalidRoles = request.Roles.Except(Roles.TenantBaseRoles, StringComparer.OrdinalIgnoreCase).ToList();

        if (invalidRoles.Any())
        {
            return Result<bool>.Validation(
                "Invalid roles selected.",
                new Dictionary<string, string[]>
                {
                    ["Roles"] = new[] { $"The following roles are not allowed for staff requests: {string.Join(", ", invalidRoles)}. Allowed roles are: {string.Join(", ", Roles.TenantBaseRoles)}." }
                });
        }

        // Verify the tenant exists and the current user is the owner
        var tenant = await uow.GetAsync<Tenant>(tenantId);
        if (tenant == null)
        {
            return Result<bool>.Validation(
                "Tenant not found.",
                new Dictionary<string, string[]>
                {
                    ["TenantId"] = new[] { $"Tenant with id {tenantId} not found." }
                });
        }

        // Check if the staff member already exists in the system
        var existingUser = await uow.QueryFirstOrDefaultAsync<AppUser>(
            "SELECT * FROM app_user WHERE email = @Email",
            new { Email = request.StaffEmail });

        if (existingUser != null && existingUser.TenantId != null)
        {
            // If user exists, check if they're already in this tenant
            if (existingUser.TenantId == tenantId)
            {
                return Result<bool>.Validation(
                    "User already exists in this tenant.",
                    new Dictionary<string, string[]>
                    {
                        ["StaffEmail"] = new[] { $"User with email {request.StaffEmail} is already a member of this tenant." }
                    });
            }

            // If user exists but in different tenant, return validation error
            return Result<bool>.Validation(
                "User already exists in another tenant.",
                new Dictionary<string, string[]>
                {
                    ["StaffEmail"] = new[] { $"User with email {request.StaffEmail} already exists in the system and belongs to another tenant." }
                });
        }

        // Check if there's already a pending request for this email in this tenant
        var existingRequest = await uow.QueryFirstOrDefaultAsync<StaffRequest>(
            "SELECT * FROM staff_request WHERE tenant_id = @TenantId AND requested_email = @Email AND status = 0",
            new { TenantId = tenantId, Email = request.StaffEmail });

        if (existingRequest != null)
        {
            return Result<bool>.Validation(
                "Pending request already exists.",
                new Dictionary<string, string[]>
                {
                    ["StaffEmail"] = new[] { $"A pending staff request for {request.StaffEmail} already exists for this tenant." }
                });
        }

        // Create the staff request
        var staffRequest = new StaffRequest
        {
            TenantId = tenantId,
            RequestedEmail = request.StaffEmail,
            RequestedName = request.StaffName,
            RequestedBySsoId = userProfile.Identifier,
            Status = StaffRequestStatus.Pending
        };

        var staffRequestId = await uow.InsertAsync(staffRequest);

        // Add roles for the staff request
        foreach (var role in request.Roles)
        {
            var staffRequestRole = new StaffRequestRole
            {
                TenantId = tenantId,
                StaffRequestId = staffRequestId,
                Role = role
            };
            await uow.InsertAsync(staffRequestRole);
        }

        return Result<bool>.Success(true, $"Staff request for {request.StaffEmail} has been created successfully. The user will be notified to accept the invitation.");
    }
}
