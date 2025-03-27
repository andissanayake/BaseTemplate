using BaseTemplate.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace BaseTemplate.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly IAuthorizationService _authorizationService;

    public IdentityService(IAuthorizationService authorizationService)
    {
        _authorizationService = authorizationService;
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
