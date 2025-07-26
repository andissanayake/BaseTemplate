using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.Application.Staff.Commands.RespondToStaffRequest;

[Authorize]
public record RespondToStaffRequestCommand : IRequest<bool>
{
    public int StaffRequestId { get; set; }
    public bool IsAccepted { get; set; }
    public string? RejectionReason { get; set; }
}

public class RespondToStaffRequestCommandHandler : IRequestHandler<RespondToStaffRequestCommand, bool>
{
    private readonly IAppDbContext _context;
    private readonly IUser _user;
    private readonly IUserProfileService _userTenantProfileService;

    public RespondToStaffRequestCommandHandler(IAppDbContext context, IUser user, IUserProfileService userTenantProfileService)
    {
        _context = context;
        _user = user;
        _userTenantProfileService = userTenantProfileService;
    }

    public async Task<Result<bool>> HandleAsync(RespondToStaffRequestCommand request, CancellationToken cancellationToken)
    {
        // Get the staff request and verify it belongs to the current user
        var staffRequest = await _context.StaffRequest
            .SingleAsync(sr => sr.Id == request.StaffRequestId && sr.RequestedEmail == _user.Email && sr.Status == StaffRequestStatus.Pending && !sr.IsDeleted, cancellationToken);

        if (request.IsAccepted)
        {
            // Check if user already has a tenant
            var user = await _context.AppUser
                .SingleAsync(u => u.SsoId == _user.Identifier && !u.TenantId.HasValue, cancellationToken);

            // Accept the request
            staffRequest.Status = StaffRequestStatus.Accepted;
            staffRequest.AcceptedAt = DateTimeOffset.UtcNow;
            staffRequest.AcceptedBySsoId = _user.Identifier;

            // Update the user's tenant information
            user.TenantId = staffRequest.TenantId;

            // Get the roles for this staff request and add them to the user
            var staffRequestRoles = await _context.StaffRequestRole
                .Where(r => r.StaffRequestId == request.StaffRequestId)
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

            await _context.SaveChangesAsync(cancellationToken);
            await _userTenantProfileService.InvalidateUserProfileCacheAsync();
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
