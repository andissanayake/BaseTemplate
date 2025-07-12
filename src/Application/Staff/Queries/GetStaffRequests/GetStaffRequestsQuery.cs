namespace BaseTemplate.Application.Staff.Queries.GetStaffRequests;

[Authorize]
public record GetStaffRequestsQuery(int TenantId) : BaseTenantRequest<List<StaffRequestDto>>(TenantId); 