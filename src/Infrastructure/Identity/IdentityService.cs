using BaseTemplate.Application.Common.Interfaces;

namespace BaseTemplate.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly IUnitOfWorkFactory _factory;
    public IdentityService(IUnitOfWorkFactory factory)
    {
        _factory = factory;
    }
    public async Task<bool> IsInRoleAsync(string userId, string role)
    {
        using var uow = _factory.CreateUOW();
        var count = await uow.QueryFirstOrDefaultAsync<int>("Select count(1) from UserRole where UserId = @userId and Role =@role", new { userId, role });
        return count != 0;
    }

    public async Task<bool> AuthorizeAsync(string userId, string policyName)
    {
        return false;
    }
}
