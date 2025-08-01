using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.Application.TenantFeatures.Staff.Queries.GetStaffInvitation;

public class GetStaffInvitationsQueryHandler(IAppDbContext context) : IRequestHandler<GetStaffInvitationsQuery, List<StaffInvitationDto>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<List<StaffInvitationDto>>> HandleAsync(GetStaffInvitationsQuery request, CancellationToken cancellationToken)
    {
        var staffRequests = await _context.StaffInvitation.AsNoTracking()
            .Include(sr => sr.RequestedByAppUser)
            .Include(sr => sr.AcceptedByAppUser)
            .OrderByDescending(sr => sr.Created)
            .ToListAsync(cancellationToken);

        // Get all roles for all staff requests in a single query (exclude soft deleted)
        var staffRequestIds = staffRequests.Select(sr => sr.Id).ToList();
        var roles = new List<StaffInvitationRole>();

        if (staffRequestIds.Count != 0)
        {
            roles = await _context.StaffInvitationRole
                .Where(r => staffRequestIds.Contains(r.StaffInvitationId))
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
                Roles = rolesByStaffRequestId.GetValueOrDefault(staffRequest.Id, []),
                RequestedByAppUserId = staffRequest.RequestedByAppUserId,
                RequestedByAppUserName = staffRequest.RequestedByAppUser.Name ?? string.Empty,
                RequestedByAppUserEmail = staffRequest.RequestedByAppUser.Email,
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
