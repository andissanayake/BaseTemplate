namespace BaseTemplate.Application.GlobalFeatures.Users.Commands.GetUser;

public record GetUserResponse
{
    public IEnumerable<string> Roles { get; set; } = [];
    public TenantDetails? Tenant { get; set; }
    public StaffInvitationDetails? StaffInvitation { get; set; }
}
