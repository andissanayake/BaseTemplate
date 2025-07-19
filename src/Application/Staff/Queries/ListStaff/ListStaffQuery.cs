using BaseTemplate.Domain.Constants;

namespace BaseTemplate.Application.Staff.Queries.ListStaff;
[Authorize(Roles = Roles.StaffManager + "," + Roles.TenantOwner)]
public record ListStaffQuery() : IRequest<List<StaffMemberDto>>;
