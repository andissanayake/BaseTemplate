using System.Security.Claims;
using BaseTemplate.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace BaseTemplate.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly IUnitOfWorkFactory _factory;
    private readonly IAuthorizationService _authorizationService;
    private readonly IUser _user;
    public IdentityService(IUnitOfWorkFactory factory, IAuthorizationService authorizationService, IUser user)
    {
        _factory = factory;
        _authorizationService = authorizationService;
        _user = user;
    }

    public bool IsAuthenticated => _user != null && _user.IsAuthenticated != null && _user.IsAuthenticated == true;
    public async Task<bool> IsInRoleAsync(string role)
    {
        if (_user == null || _user.Identifier == null)
        {
            return false;
        }
        using var uow = _factory.Create();
        var count = await uow.QueryFirstOrDefaultAsync<int>("select count(1) from user_role where user_sso_id = @Identifier and role =@role", new { _user.Identifier, role });
        return count != 0;
    }
    public async Task<bool> AuthorizeAsync(string policyName)
    {
        if (_user == null || _user.Identifier == null)
        {
            return false;
        }
        using var uow = _factory.Create();
        var roles = await uow.QueryAsync<string>("select role from user_role where user_sso_id = @Identifier", new { _user.Identifier });
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, _user.Identifier)
        };
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }
        var identity = new ClaimsIdentity(claims, "Custom");
        var principal = new ClaimsPrincipal(identity);
        var result = await _authorizationService.AuthorizeAsync(principal, policyName);
        return result.Succeeded;
    }
}
