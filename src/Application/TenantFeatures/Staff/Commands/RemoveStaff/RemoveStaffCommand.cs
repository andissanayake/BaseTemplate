namespace BaseTemplate.Application.TenantFeatures.Staff.Commands.RemoveStaff;

[Authorize(Roles = Roles.StaffManager)]
public record RemoveStaffCommand(int StaffId) : IRequest<bool>;
