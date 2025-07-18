namespace BaseTemplate.Application.Staff.Queries.GetStaffRequests;

public class GetStaffRequestsQueryHandler : IRequestHandler<GetStaffRequestsQuery, List<StaffRequestDto>>
{
    private readonly IUnitOfWorkFactory _factory;
    private readonly IUserProfileService _userProfileService;

    public GetStaffRequestsQueryHandler(IUnitOfWorkFactory factory, IUserProfileService userProfileService)
    {
        _factory = factory;
        _userProfileService = userProfileService;
    }

    public async Task<Result<List<StaffRequestDto>>> HandleAsync(GetStaffRequestsQuery request, CancellationToken cancellationToken)
    {
        // Get user profile to get tenant ID
        var userProfile = await _userProfileService.GetUserProfileAsync();
        if (userProfile?.TenantId == null)
        {
            return Result<List<StaffRequestDto>>.Forbidden("User is not associated with any tenant.");
        }

        var tenantId = userProfile.TenantId.Value;

        using var uow = _factory.Create();
        
        // Get all staff requests for the tenant
        var staffRequests = await uow.QueryAsync<StaffRequest>(
            "SELECT * FROM staff_request WHERE tenant_id = @TenantId ORDER BY created DESC",
            new { TenantId = tenantId });
            
        // Get all roles for all staff requests in a single query
        var staffRequestIds = staffRequests.Select(sr => sr.Id).ToList();
        var roles = new List<StaffRequestRole>();
        
        if (staffRequestIds.Any())
        {
            var roleQuery = "SELECT * FROM staff_request_role WHERE tenant_id = @TenantId AND staff_request_id = ANY(@StaffRequestIds)";
            roles = (await uow.QueryAsync<StaffRequestRole>(roleQuery, new { TenantId = tenantId, StaffRequestIds = staffRequestIds })).ToList();
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