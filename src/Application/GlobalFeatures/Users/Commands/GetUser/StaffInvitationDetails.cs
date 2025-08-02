namespace BaseTemplate.Application.GlobalFeatures.Users.Commands.GetUser;

public record StaffInvitationDetails
{
    public int Id { get; set; }
    public string RequesterName { get; set; } = string.Empty;
    public string RequesterEmail { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = [];
    public StaffInvitationStatus Status { get; set; }
    public DateTimeOffset Created { get; set; }
    public string TenantName { get; set; } = string.Empty;
}
