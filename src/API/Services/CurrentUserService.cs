using System.Security.Claims;

using BaseTemplate.Application.Common.Interfaces;

namespace BaseTemplate.API.Services;

public class CurrentUserService : IUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    public string Identifier => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier)!;
    public string Name => _httpContextAccessor.HttpContext?.User?.FindFirstValue("name")!;
    public string Email => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email)!;
}
