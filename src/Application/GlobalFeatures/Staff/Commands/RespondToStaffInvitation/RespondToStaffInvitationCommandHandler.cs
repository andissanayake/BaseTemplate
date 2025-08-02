using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.Application.GlobalFeatures.Staff.Commands.RespondToStaffInvitation;
public class RespondToStaffInvitationCommandHandler(IBaseDbContext context, IUser user) : IRequestHandler<RespondToStaffInvitationCommand, bool>
{
    private readonly IBaseDbContext _context = context;
    private readonly IUser _user = user;

    public async Task<Result<bool>> HandleAsync(RespondToStaffInvitationCommand request, CancellationToken cancellationToken)
    {
        var staffRequest = await _context.StaffInvitation
            .SingleAsync(sr =>
                sr.Id == request.StaffInvitationId &&
                sr.RequestedEmail == _user.Email &&
                sr.Status == StaffInvitationStatus.Pending,
                cancellationToken);

        if (request.IsAccepted)
        {
            var user = await _context.AppUser
                .SingleAsync(u =>
                    u.SsoId == _user.Identifier &&
                    !u.TenantId.HasValue,
                    cancellationToken);

            staffRequest.Status = StaffInvitationStatus.Accepted;
            staffRequest.AcceptedAt = DateTimeOffset.UtcNow;
            staffRequest.AcceptedByAppUserId = user.Id;

            var staffRequestRoles = await _context.StaffInvitationRole
                .Where(r => r.StaffInvitationId == request.StaffInvitationId)
                .ToListAsync(cancellationToken);

            if (staffRequestRoles.Count != 0)
            {
                _context.UserRole.AddRange(staffRequestRoles.Select(
                    role => new UserRole { UserId = user.Id, Role = role.Role }));
            }
            user.TenantId = staffRequest.TenantId;
            await _context.SaveChangesAsync(cancellationToken);
            return Result<bool>.Success(true, $"You have successfully accepted the staff request.");
        }
        else
        {
            if (string.IsNullOrWhiteSpace(request.RejectionReason))
            {
                return Result<bool>.Validation(
                    "Rejection reason is required when rejecting a staff request.",
                    new Dictionary<string, string[]>
                    {
                        ["RejectionReason"] = ["Please provide a reason for rejecting this staff request."]
                    });
            }
            staffRequest.Status = StaffInvitationStatus.Rejected;
            staffRequest.RejectionReason = request.RejectionReason;
            await _context.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true, "Staff request has been rejected successfully.");
        }
    }
}
