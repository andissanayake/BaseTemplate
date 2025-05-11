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
        if (_user == null || _user.Id == null)
        {
            return false;
        }
        using var uow = _factory.Create();
        var count = await uow.QueryFirstOrDefaultAsync<int>("Select count(1) from UserRole where UserId = @userId and Role =@role", new { userId = _user.Id, role });
        return count != 0;
    }
    public async Task<bool> AuthorizeAsync(string policyName)
    {
        if (_user == null || _user.Id == null)
        {
            return false;
        }
        using var uow = _factory.Create();
        var roles = await uow.QueryAsync<string>("Select Role from UserRole where UserId = @userId", new { userId = _user.Id });
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, _user.Id)
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
    public async Task<IEnumerable<string>> GetRolesAsync()
    {
        if (_user == null || _user.Id == null)
        {
            return [];
        }
        using var uow = _factory.Create();
        return await uow.QueryAsync<string>("Select Role from UserRole where UserId = @userId", new { userId = _user.Id });
    }
}
