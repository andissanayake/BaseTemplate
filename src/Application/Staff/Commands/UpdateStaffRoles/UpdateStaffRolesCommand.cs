using BaseTemplate.Domain.Constants;

namespace BaseTemplate.Application.Staff.Commands.UpdateStaffRoles;

[Authorize(Roles = Roles.StaffManager + "," + Roles.TenantOwner)]
public record UpdateStaffRolesCommand(int TenantId, int StaffId, List<string> NewRoles) : BaseTenantRequest<bool>(TenantId); 