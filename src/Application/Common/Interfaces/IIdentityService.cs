namespace BaseTemplate.Application.Common.Interfaces;

public interface IIdentityService
{
    bool IsAuthenticated { get; }
    Task<bool> IsInRoleAsync(string role);
    Task<bool> AuthorizeAsync(string policyName);
    Task<bool> HasTenantAccessAsync(int tenantId);
}
