using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.Application.TenantFeatures.Staff.Commands.CreateStaffInvitation;

public class CreateStaffInvitationCommandHandler(IAppDbContext context) : IRequestHandler<CreateStaffInvitationCommand, bool>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<bool>> HandleAsync(CreateStaffInvitationCommand request, CancellationToken cancellationToken)
    {
        // Validate that only allowed roles can be requested
        var invalidRoles = request.Roles.Except(Roles.TenantBaseRoles, StringComparer.OrdinalIgnoreCase).ToList();
        if (invalidRoles.Count != 0)
        {
            return Result<bool>.Validation(
                "Invalid roles selected.",
                new Dictionary<string, string[]>
                {
                    ["Roles"] = [$"The following roles are not allowed for staff requests: {string.Join(", ", invalidRoles)}. Allowed roles are: {string.Join(", ", Roles.TenantBaseRoles)}."]
                });
        }

        // Check if the staff member already exists in the system
        var existingUser = await _context.AppUser
            .FirstOrDefaultAsync(u => u.Email.Trim().ToLowerInvariant() == request.StaffEmail.Trim().ToLowerInvariant(), cancellationToken);
        if (existingUser != null && existingUser.TenantId.HasValue)
        {
            return Result<bool>.Validation(
                "User already exists in another tenant.",
                new Dictionary<string, string[]>
                {
                    ["StaffEmail"] = [$"User with email {request.StaffEmail} already exists in the system and belongs to another tenant."]
                });
        }

        // Check if there's already a pending request for this email in this tenant
        var existingInvitation = await _context.StaffInvitation.AsNoTracking()
            .AnyAsync(r => r.RequestedEmail.Trim().ToLowerInvariant() == request.StaffEmail.Trim().ToLowerInvariant() && r.Status == StaffInvitationStatus.Pending, cancellationToken);

        if (existingInvitation)
        {
            return Result<bool>.Validation(
                "Pending request already exists.",
                new Dictionary<string, string[]>
                {
                    ["StaffEmail"] = [$"A pending staff request for {request.StaffEmail} already exists for this tenant."]
                });
        }

        var staffInvitation = new StaffInvitation
        {
            RequestedEmail = request.StaffEmail,
            RequestedName = request.StaffName,
            Status = StaffInvitationStatus.Pending,
            StaffInvitationRoles = [],

        };

        // Add roles for the staff request
        staffInvitation.StaffInvitationRoles.AddRange(
            request.Roles.Select(srr => new StaffInvitationRole() { Role = srr })
            );
        _context.StaffInvitation.Add(staffInvitation);
        await _context.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true, $"Staff request for {request.StaffEmail} has been created successfully. The user will be notified to accept the invitation.");
    }
}
