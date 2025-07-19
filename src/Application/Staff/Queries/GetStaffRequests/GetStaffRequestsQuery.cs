namespace BaseTemplate.Application.Staff.Queries.GetStaffRequests;
[Authorize(Roles = Roles.StaffManager)]
public record GetStaffRequestsQuery() : IRequest<List<StaffRequestDto>>;
