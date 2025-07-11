using System.ComponentModel.DataAnnotations;
namespace BaseTemplate.Application.Tenants.Commands.RequestStaff;
[Authorize(Roles = Domain.Constants.Roles.StaffRequestManager + "," + Domain.Constants.Roles.TenantOwner)]
public record RequestStaffCommand(int TenantId) : BaseTenantRequest<bool>(TenantId)
{
    [Required]
    [EmailAddress(ErrorMessage = "Please provide a valid email address.")]
    public string StaffEmail { get; set; } = string.Empty;

    [Required]
    [MinLength(2, ErrorMessage = "Staff name must be at least 2 characters long.")]
    [MaxLength(100, ErrorMessage = "Staff name cannot exceed 100 characters.")]
    public string StaffName { get; set; } = string.Empty;

    [Required]
    [MinLength(1, ErrorMessage = "At least one role must be selected.")]
    public List<string> Roles { get; set; } = new();
}

public class RequestStaffCommandHandler : IRequestHandler<RequestStaffCommand, bool>
{
    private readonly IUnitOfWorkFactory _factory;
    private readonly IUser _user;

    public RequestStaffCommandHandler(IUnitOfWorkFactory factory, IUser user)
    {
        _factory = factory;
        _user = user;
    }

    public async Task<Result<bool>> HandleAsync(RequestStaffCommand request, CancellationToken cancellationToken)
    {
        using var uow = _factory.Create();

        // Validate that only allowed roles can be requested
        var allowedRoles = new[] { "ItemManager", "StaffRequestManager", "TenantManager" };
        var invalidRoles = request.Roles.Except(allowedRoles, StringComparer.OrdinalIgnoreCase).ToList();

        if (invalidRoles.Any())
        {
            return Result<bool>.Validation(
                "Invalid roles selected.",
                new Dictionary<string, string[]>
                {
                    ["Roles"] = new[] { $"The following roles are not allowed for staff requests: {string.Join(", ", invalidRoles)}. Allowed roles are: {string.Join(", ", allowedRoles)}." }
                });
        }

        // Verify the tenant exists and the current user is the owner
        var tenant = await uow.GetAsync<Tenant>(request.TenantId);
        if (tenant == null)
        {
            return Result<bool>.Validation(
                "Tenant not found.",
                new Dictionary<string, string[]>
                {
                    ["TenantId"] = new[] { $"Tenant with id {request.TenantId} not found." }
                });
        }

        if (tenant.OwnerSsoId != _user.Identifier)
        {
            return Result<bool>.Validation(
                "Access denied.",
                new Dictionary<string, string[]>
                {
                    ["TenantId"] = new[] { "Only tenant owners can request staff members." }
                });
        }

        // Check if the staff member already exists in the system
        var existingUser = await uow.QueryFirstOrDefaultAsync<AppUser>(
            "SELECT * FROM app_user WHERE email = @Email",
            new { Email = request.StaffEmail });

        if (existingUser != null)
        {
            // If user exists, check if they're already in this tenant
            if (existingUser.TenantId == request.TenantId)
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
            new { TenantId = request.TenantId, Email = request.StaffEmail });

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
            TenantId = request.TenantId,
            RequestedEmail = request.StaffEmail,
            RequestedName = request.StaffName,
            RequestedBySsoId = _user.Identifier,
            Status = StaffRequestStatus.Pending
        };

        var staffRequestId = await uow.InsertAsync(staffRequest);

        // Add roles for the staff request
        foreach (var role in request.Roles)
        {
            var staffRequestRole = new StaffRequestRole
            {
                TenantId = request.TenantId,
                StaffRequestId = staffRequestId,
                Role = role
            };
            await uow.InsertAsync(staffRequestRole);
        }

        return Result<bool>.Success(true, $"Staff request for {request.StaffEmail} has been created successfully. The user will be notified to accept the invitation.");
    }
}
