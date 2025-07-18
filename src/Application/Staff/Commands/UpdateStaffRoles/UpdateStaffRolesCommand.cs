using BaseTemplate.Domain.Constants;

namespace BaseTemplate.Application.Staff.Commands.UpdateStaffRoles;

[Authorize(Roles = Roles.StaffManager + "," + Roles.TenantOwner)]
public record UpdateStaffRolesCommand(int StaffId, List<string> NewRoles) : IRequest<bool>; 