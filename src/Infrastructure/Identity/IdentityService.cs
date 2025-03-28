using BaseTemplate.Application.Common.Interfaces;

namespace BaseTemplate.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    public IdentityService()
    {
    }
    public async Task<bool> IsInRoleAsync(string userId, string role)
    {
        return false;
    }

    public async Task<bool> AuthorizeAsync(string userId, string policyName)
    {
        return false;
    }
}
