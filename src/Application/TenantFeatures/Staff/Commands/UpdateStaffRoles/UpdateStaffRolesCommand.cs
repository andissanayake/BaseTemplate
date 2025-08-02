namespace BaseTemplate.Application.TenantFeatures.Staff.Commands.UpdateStaffRoles;

[Authorize(Roles = Roles.StaffManager)]
public record UpdateStaffRolesCommand(int StaffId, List<string> NewRoles) : IRequest<bool>;
