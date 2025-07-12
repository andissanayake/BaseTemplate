using BaseTemplate.Domain.Constants;

namespace BaseTemplate.Application.Staff.Commands.RemoveStaff;

[Authorize(Roles = Roles.StaffManager + "," + Roles.TenantOwner)]
public record RemoveStaffCommand(int TenantId, string StaffSsoId) : BaseTenantRequest<bool>(TenantId); 