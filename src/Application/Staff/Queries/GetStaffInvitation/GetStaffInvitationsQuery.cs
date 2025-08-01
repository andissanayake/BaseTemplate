namespace BaseTemplate.Application.Staff.Queries.GetStaffInvitation;
[Authorize(Roles = Roles.StaffInvitationManager)]
public record GetStaffInvitationsQuery() : IRequest<List<StaffInvitationDto>>;
