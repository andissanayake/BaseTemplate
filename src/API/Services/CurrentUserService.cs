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

    public string? Id => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
    public string? UserName => _httpContextAccessor.HttpContext?.User?.Identity?.Name;
    public bool? IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated;
}
