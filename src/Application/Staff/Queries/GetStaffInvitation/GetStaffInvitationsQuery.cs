namespace BaseTemplate.Application.Staff.Queries.GetStaffInvitation;
[Authorize(Roles = Roles.StaffRequestManager)]
public record GetStaffInvitationsQuery() : IRequest<List<StaffInvitationDto>>;
