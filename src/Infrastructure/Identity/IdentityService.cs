using System.Security.Claims;
using BaseTemplate.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace BaseTemplate.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly IUnitOfWorkFactory _factory;
    private readonly IAuthorizationService _authorizationService;
    public IdentityService(IUnitOfWorkFactory factory, IAuthorizationService authorizationService)
    {
        _factory = factory;
        _authorizationService = authorizationService;
    }
    public async Task<bool> IsInRoleAsync(string userId, string role)
    {
        using var uow = _factory.CreateUOW();
        var count = await uow.QueryFirstOrDefaultAsync<int>("Select count(1) from UserRole where UserId = @userId and Role =@role", new { userId, role });
        return count != 0;
    }

    public async Task<bool> AuthorizeAsync(string userId, string policyName)
    {
        using var uow = _factory.CreateUOW();
        var roles = await uow.QueryAsync<string>("Select Role from UserRole where UserId = @userId", new { userId });
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId)
        };
        foreach (var role in roles)
        {
            claims.Add(new Claim("role", role));
        }
        var identity = new ClaimsIdentity(claims, "Custom");
        var principal = new ClaimsPrincipal(identity);
        var result = await _authorizationService.AuthorizeAsync(principal, policyName);
        return result.Succeeded;
    }
}
