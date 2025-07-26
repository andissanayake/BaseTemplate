using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.Application.Staff.Commands.RequestStaff;

public class RequestStaffCommandHandler : IRequestHandler<RequestStaffCommand, bool>
{
    private readonly IAppDbContext _context;
    private readonly IUserProfileService _userProfileService;

    public RequestStaffCommandHandler(IAppDbContext context, IUserProfileService userProfileService)
    {
        _context = context;
        _userProfileService = userProfileService;
    }

    public async Task<Result<bool>> HandleAsync(RequestStaffCommand request, CancellationToken cancellationToken)
    {
        var userProfile = await _userProfileService.GetUserProfileAsync();

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

        // Check if the staff member already exists in the system
        var existingUser = await _context.AppUser
            .FirstOrDefaultAsync(u => u.Email == request.StaffEmail, cancellationToken);
        if (existingUser != null && existingUser.TenantId != null)
        {
            if (existingUser.TenantId == userProfile.TenantId)
            {
                return Result<bool>.Validation(
                    "User already exists in this tenant.",
                    new Dictionary<string, string[]>
                    {
                        ["StaffEmail"] = new[] { $"User with email {request.StaffEmail} is already a member of this tenant." }
                    });
            }
            return Result<bool>.Validation(
                "User already exists in another tenant.",
                new Dictionary<string, string[]>
                {
                    ["StaffEmail"] = new[] { $"User with email {request.StaffEmail} already exists in the system and belongs to another tenant." }
                });
        }

        // Check if there's already a pending request for this email in this tenant
        var existingRequest = await _context.StaffRequest
            .FirstOrDefaultAsync(r => r.TenantId == userProfile.TenantId && r.RequestedEmail == request.StaffEmail && r.Status == StaffRequestStatus.Pending, cancellationToken);

        if (existingRequest != null)
        {
            return Result<bool>.Validation(
                "Pending request already exists.",
                new Dictionary<string, string[]>
                {
                    ["StaffEmail"] = new[] { $"A pending staff request for {request.StaffEmail} already exists for this tenant." }
                });
        }

        var staffRequest = new StaffRequest
        {
            TenantId = userProfile.TenantId,
            RequestedEmail = request.StaffEmail,
            RequestedName = request.StaffName,
            RequestedByAppUserId = userProfile.Id,
            Status = StaffRequestStatus.Pending
        };
        _context.StaffRequest.Add(staffRequest);
        
        // Save the staff request first to get its ID
        await _context.SaveChangesAsync(cancellationToken);

        // Add roles for the staff request
        foreach (var role in request.Roles)
        {
            var staffRequestRole = new StaffRequestRole
            {
                TenantId = userProfile.TenantId,
                StaffRequestId = staffRequest.Id,
                Role = role
            };
            _context.StaffRequestRole.Add(staffRequestRole);
        }
        await _context.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true, $"Staff request for {request.StaffEmail} has been created successfully. The user will be notified to accept the invitation.");
    }
}
