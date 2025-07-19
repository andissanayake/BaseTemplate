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
    private readonly IUnitOfWorkFactory _factory;
    private readonly IUser _user;
    private readonly IUserTenantProfileService _userTenantProfileService;

    public RespondToStaffRequestCommandHandler(IUnitOfWorkFactory factory, IUser user, IUserTenantProfileService userTenantProfileService)
    {
        _factory = factory;
        _user = user;
        _userTenantProfileService = userTenantProfileService;
    }

    public async Task<Result<bool>> HandleAsync(RespondToStaffRequestCommand request, CancellationToken cancellationToken)
    {
        using var uow = _factory.Create();

        // Get the staff request and verify it belongs to the current user
        var staffRequest = await uow.QuerySingleAsync<StaffRequest>(
            "SELECT * FROM staff_request WHERE id = @Id AND requested_email = @Email",
            new { Id = request.StaffRequestId, _user.Email });

        if (staffRequest.Status != StaffRequestStatus.Pending)
        {
            return Result<bool>.Validation("This staff request has already been processed.");
        }

        if (request.IsAccepted)
        {
            // Use transaction for accepting the request
            using var transaction = uow.BeginTransaction();

            // Accept the request
            staffRequest.Status = StaffRequestStatus.Accepted;
            staffRequest.AcceptedAt = DateTimeOffset.UtcNow;
            staffRequest.AcceptedBySsoId = _user.Identifier;
            await uow.UpdateAsync(staffRequest);

            // Update the user's tenant information
            var user = await uow.QuerySingleAsync<AppUser>(
                "SELECT * FROM app_user WHERE sso_id = @SsoId",
                new { SsoId = _user.Identifier });

            user.TenantId = staffRequest.TenantId;
            await uow.UpdateAsync(user);

            // Get the roles for this staff request and add them to the user
            var staffRequestRoles = await uow.QueryAsync<StaffRequestRole>(
                "SELECT * FROM staff_request_role WHERE staff_request_id = @StaffRequestId",
                new { request.StaffRequestId });

            foreach (var role in staffRequestRoles)
            {
                var userRole = new UserRole
                {
                    UserId = user.Id,
                    Role = role.Role
                };
                await uow.InsertAsync(userRole);
            }
            await _userTenantProfileService.InvalidateUserProfileCacheAsync();
            transaction.Commit();
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

            // Use transaction for rejecting the request
            using var transaction = uow.BeginTransaction();

            staffRequest.Status = StaffRequestStatus.Rejected;
            staffRequest.RejectionReason = request.RejectionReason;
            await uow.UpdateAsync(staffRequest);

            transaction.Commit();
            return Result<bool>.Success(true, "Staff request has been rejected successfully.");
        }
    }
}
