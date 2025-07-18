using BaseTemplate.Domain.Constants;

namespace BaseTemplate.Application.Staff.Commands.RemoveStaff;

[Authorize(Roles = Roles.StaffManager)]
public record RemoveStaffCommand(int StaffId) : IRequest<bool>;
