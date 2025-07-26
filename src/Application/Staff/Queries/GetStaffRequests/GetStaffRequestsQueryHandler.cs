using Microsoft.EntityFrameworkCore;

namespace BaseTemplate.Application.Staff.Queries.GetStaffRequests;

public class GetStaffRequestsQueryHandler : IRequestHandler<GetStaffRequestsQuery, List<StaffRequestDto>>
{
    private readonly IAppDbContext _context;
    private readonly IUserProfileService _userProfileService;

    public GetStaffRequestsQueryHandler(IAppDbContext context, IUserProfileService userProfileService)
    {
        _context = context;
        _userProfileService = userProfileService;
    }

    public async Task<Result<List<StaffRequestDto>>> HandleAsync(GetStaffRequestsQuery request, CancellationToken cancellationToken)
    {
        var userProfile = await _userProfileService.GetUserProfileAsync();

        var staffRequests = await _context.StaffRequest
            .Where(sr => sr.TenantId == userProfile.TenantId && !sr.IsDeleted)
            .OrderByDescending(sr => sr.Created)
            .ToListAsync(cancellationToken);

        // Get all roles for all staff requests in a single query (exclude soft deleted)
        var staffRequestIds = staffRequests.Select(sr => sr.Id).ToList();
        var roles = new List<StaffRequestRole>();

        if (staffRequestIds.Any())
        {
            roles = await _context.StaffRequestRole
                .Where(r => r.TenantId == userProfile.TenantId && staffRequestIds.Contains(r.StaffRequestId) && !r.IsDeleted)
                .ToListAsync(cancellationToken);
        }

        // Group roles by staff request ID for efficient lookup
        var rolesByStaffRequestId = roles.GroupBy(r => r.StaffRequestId).ToDictionary(g => g.Key, g => g.Select(r => r.Role).ToList());

        var result = new List<StaffRequestDto>();

        foreach (var staffRequest in staffRequests)
        {
            var dto = new StaffRequestDto
            {
                Id = staffRequest.Id,
                RequestedEmail = staffRequest.RequestedEmail,
                RequestedName = staffRequest.RequestedName,
                Roles = rolesByStaffRequestId.GetValueOrDefault(staffRequest.Id, new List<string>()),
                RequestedBySsoId = staffRequest.RequestedBySsoId,
                Status = staffRequest.Status,
                Created = staffRequest.Created,
                AcceptedAt = staffRequest.AcceptedAt,
                AcceptedBySsoId = staffRequest.AcceptedBySsoId,
                RejectionReason = staffRequest.RejectionReason
            };

            result.Add(dto);
        }

        return Result<List<StaffRequestDto>>.Success(result);
    }
}
