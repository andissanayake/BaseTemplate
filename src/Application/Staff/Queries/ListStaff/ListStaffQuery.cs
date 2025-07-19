namespace BaseTemplate.Application.Staff.Queries.ListStaff;
[Authorize(Roles = Roles.StaffManager)]
public record ListStaffQuery() : IRequest<List<StaffMemberDto>>;
