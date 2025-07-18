namespace BaseTemplate.Application.Staff.Queries.GetStaffRequests;
[Authorize]
public record GetStaffRequestsQuery() : IRequest<List<StaffRequestDto>>;
