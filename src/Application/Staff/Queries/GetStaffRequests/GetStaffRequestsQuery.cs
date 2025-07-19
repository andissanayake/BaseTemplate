namespace BaseTemplate.Application.Staff.Queries.GetStaffRequests;
[Authorize(Roles = Roles.StaffRequestManager)]
public record GetStaffRequestsQuery() : IRequest<List<StaffRequestDto>>;
