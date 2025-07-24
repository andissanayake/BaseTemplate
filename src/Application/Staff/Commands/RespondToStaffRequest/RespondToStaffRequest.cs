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
            .FirstOrDefaultAsync(sr => sr.Id == request.StaffRequestId && sr.RequestedEmail == _user.Email, cancellationToken);
        if (staffRequest == null)
        {
            return Result<bool>.NotFound($"Staff request with id {request.StaffRequestId} not found for this user.");
        }
        if (staffRequest.Status != StaffRequestStatus.Pending)
        {
            return Result<bool>.Validation("This staff request has already been processed.");
        }
        if (request.IsAccepted)
        {
            // Accept the request
            staffRequest.Status = StaffRequestStatus.Accepted;
            staffRequest.AcceptedAt = DateTimeOffset.UtcNow;
            staffRequest.AcceptedBySsoId = _user.Identifier;

            // Update the user's tenant information
            var user = await _context.AppUser
                .FirstOrDefaultAsync(u => u.SsoId == _user.Identifier, cancellationToken);
            if (user == null)
            {
                return Result<bool>.NotFound("User not found.");
            }
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
            await _userTenantProfileService.InvalidateUserProfileCacheAsync();
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
