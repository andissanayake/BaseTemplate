namespace BaseTemplate.Application.Staff.Commands.UpdateStaffRoles;

[Authorize(Roles = Roles.StaffManager)]
public record UpdateStaffRolesCommand(int StaffId, List<string> NewRoles) : IRequest<bool>;
