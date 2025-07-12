namespace BaseTemplate.Application.Staff.Queries.GetStaffRequests;

[Authorize]
public record GetStaffRequestsQuery(int TenantId) : BaseTenantRequest<List<StaffRequestDto>>(TenantId);

public class StaffRequestDto
{
    public int Id { get; set; }
    public string RequestedEmail { get; set; } = string.Empty;
    public string RequestedName { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
    public string RequestedBySsoId { get; set; } = string.Empty;
    public StaffRequestStatus Status { get; set; }
    public DateTimeOffset Created { get; set; }
    public DateTimeOffset? AcceptedAt { get; set; }
    public string? AcceptedBySsoId { get; set; }
    public string? RejectionReason { get; set; }
}

public class GetStaffRequestsQueryHandler : IRequestHandler<GetStaffRequestsQuery, List<StaffRequestDto>>
{
    private readonly IUnitOfWorkFactory _factory;

    public GetStaffRequestsQueryHandler(IUnitOfWorkFactory factory)
    {
        _factory = factory;
    }

    public async Task<Result<List<StaffRequestDto>>> HandleAsync(GetStaffRequestsQuery request, CancellationToken cancellationToken)
    {
        using var uow = _factory.Create();
        
        // Get all staff requests for the tenant
        var staffRequests = await uow.QueryAsync<StaffRequest>(
            "SELECT * FROM staff_request WHERE tenant_id = @TenantId ORDER BY created DESC",
            new { request.TenantId });
            
        // Get all roles for all staff requests in a single query
        var staffRequestIds = staffRequests.Select(sr => sr.Id).ToList();
        var roles = new List<StaffRequestRole>();
        
        if (staffRequestIds.Any())
        {
            var roleQuery = "SELECT * FROM staff_request_role WHERE tenant_id = @TenantId AND staff_request_id = ANY(@StaffRequestIds)";
            roles = (await uow.QueryAsync<StaffRequestRole>(roleQuery, new { request.TenantId, StaffRequestIds = staffRequestIds })).ToList();
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