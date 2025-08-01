using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.Application.Staff.Commands.RespondToStaffInvitation;
public class RespondToStaffInvitationCommandHandler : IRequestHandler<RespondToStaffInvitationCommand, bool>
{
    private readonly IBaseDbContext _context;
    private readonly IUser _user;

    public RespondToStaffInvitationCommandHandler(IBaseDbContext context, IUser user)
    {
        _context = context;
        _user = user;
    }

    public async Task<Result<bool>> HandleAsync(RespondToStaffInvitationCommand request, CancellationToken cancellationToken)
    {
        // Get the staff request and verify it belongs to the current user
        var staffRequest = await _context.StaffInvitation
            .SingleAsync(sr => sr.Id == request.StaffRequestId && sr.RequestedEmail == _user.Email && sr.Status == StaffRequestStatus.Pending, cancellationToken);

        if (request.IsAccepted)
        {
            // Check if user already has a tenant
            var user = await _context.AppUser
                .SingleAsync(u => u.SsoId == _user.Identifier && !u.TenantId.HasValue, cancellationToken);

            // Accept the request
            staffRequest.Status = StaffRequestStatus.Accepted;
            staffRequest.AcceptedAt = DateTimeOffset.UtcNow;
            staffRequest.AcceptedByAppUserId = user.Id;

            // Get the roles for this staff request and add them to the user
            var staffRequestRoles = await _context.StaffInvitationRole
                .Where(r => r.StaffInvitationId == request.StaffRequestId)
                .ToListAsync(cancellationToken);

            foreach (var role in staffRequestRoles)
            {
                var userRole = new UserRole
                {
                    UserId = user.Id,
                    Role = role.Role
                };
                _context.UserRole.Add(userRole);
            }

            // Update the user's tenant information
            user.TenantId = staffRequest.TenantId;
            _context.AppUser.Update(user);

            await _context.SaveChangesAsync(cancellationToken);
            return Result<bool>.Success(true, $"You have successfully accepted the staff request.");
        }
        else
        {
            // Reject the request
            if (string.IsNullOrWhiteSpace(request.RejectionReason))
            {
                return Result<bool>.Validation(
                    "Rejection reason is required when rejecting a staff request.",
                    new Dictionary<string, string[]>
                    {
                        ["RejectionReason"] = new[] { "Please provide a reason for rejecting this staff request." }
                    });
            }
            staffRequest.Status = StaffRequestStatus.Rejected;
            staffRequest.RejectionReason = request.RejectionReason;
            await _context.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true, "Staff request has been rejected successfully.");
        }
    }
}
