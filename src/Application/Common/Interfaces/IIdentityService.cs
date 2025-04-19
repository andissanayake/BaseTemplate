namespace BaseTemplate.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<bool> IsInRoleAsync(string role);

    Task<bool> AuthorizeAsync(string policyName);

    Task<IEnumerable<string>> GetRolesAsync();
}
