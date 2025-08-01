using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.Application.Staff.Queries.GetStaffInvitation;

public class GetStaffInvitationsQueryHandler : IRequestHandler<GetStaffInvitationsQuery, List<StaffInvitationDto>>
{
    private readonly IAppDbContext _context;
    private readonly IUserProfileService _userProfileService;

    public GetStaffInvitationsQueryHandler(IAppDbContext context, IUserProfileService userProfileService)
    {
        _context = context;
        _userProfileService = userProfileService;
    }

    public async Task<Result<List<StaffInvitationDto>>> HandleAsync(GetStaffInvitationsQuery request, CancellationToken cancellationToken)
    {
        var userProfile = await _userProfileService.GetUserProfileAsync();

        var staffRequests = await _context.StaffInvitation
            .Include(sr => sr.RequestedByAppUser)
            .Include(sr => sr.AcceptedByAppUser)
            .Where(sr => sr.TenantId == userProfile.TenantId && !sr.IsDeleted)
            .OrderByDescending(sr => sr.Created)
            .ToListAsync(cancellationToken);

        // Get all roles for all staff requests in a single query (exclude soft deleted)
        var staffRequestIds = staffRequests.Select(sr => sr.Id).ToList();
        var roles = new List<StaffInvitationRole>();

        if (staffRequestIds.Any())
        {
            roles = await _context.StaffInvitationRole
                .Where(r => r.TenantId == userProfile.TenantId && staffRequestIds.Contains(r.StaffInvitationId) && !r.IsDeleted)
                .ToListAsync(cancellationToken);
        }

        // Group roles by staff request ID for efficient lookup
        var rolesByStaffRequestId = roles.GroupBy(r => r.StaffInvitationId).ToDictionary(g => g.Key, g => g.Select(r => r.Role).ToList());

        var result = new List<StaffInvitationDto>();

        foreach (var staffRequest in staffRequests)
        {
            var dto = new StaffInvitationDto
            {
                Id = staffRequest.Id,
                RequestedEmail = staffRequest.RequestedEmail,
                RequestedName = staffRequest.RequestedName,
                Roles = rolesByStaffRequestId.GetValueOrDefault(staffRequest.Id, new List<string>()),
                RequestedByAppUserId = staffRequest.RequestedByAppUserId,
                RequestedByAppUserName = staffRequest.RequestedByAppUser.Name ?? string.Empty,
                RequestedByAppUserEmail = staffRequest.RequestedByAppUser.Email ?? string.Empty,
                Status = staffRequest.Status,
                Created = staffRequest.Created,
                AcceptedAt = staffRequest.AcceptedAt,
                AcceptedByAppUserId = staffRequest.AcceptedByAppUserId,
                AcceptedByAppUserName = staffRequest.AcceptedByAppUser?.Name,
                AcceptedByAppUserEmail = staffRequest.AcceptedByAppUser?.Email,
                RejectionReason = staffRequest.RejectionReason
            };

            result.Add(dto);
        }

        return Result<List<StaffInvitationDto>>.Success(result);
    }
}
