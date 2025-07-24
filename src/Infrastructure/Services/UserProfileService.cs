using BaseTemplate.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace BaseTemplate.Infrastructure.Services
{
    public class UserProfileService : IUserProfileService
    {
        private readonly IAppDbContext _context;
        private readonly IMemoryCache _cache;
        private readonly IUser _user;

        public UserProfileService(IAppDbContext context, IMemoryCache cache, IUser user)
        {
            _context = context;
            _cache = cache;
            _user = user;
        }

        public async Task<UserProfileDto> GetUserProfileAsync()
        {
            return await GetUserProfileByIdentifierInternalAsync(_user.Identifier!);
        }

        private async Task<UserProfileDto> GetUserProfileByIdentifierInternalAsync(string identifier)
        {
            var cacheKey = $"user_profile:{identifier}";
            if (_cache.TryGetValue(cacheKey, out UserProfileDto? cachedProfile))
            {
                if (cachedProfile != null) return cachedProfile;
            }

            var appUser = await _context.AppUser.SingleAsync(u => u.SsoId == identifier);
            var tenant = await _context.Tenant.SingleAsync(t => t.Id == appUser.TenantId);
            var roles = _context.UserRole.Where(r => r.UserId == appUser.Id).ToList();

            var userInfo = new UserProfileDto()
            {
                Email = appUser.Email!,
                Id = appUser.Id,
                Identifier = appUser.SsoId,
                Name = appUser.Name!,
                Roles = roles.Select(r => r.Role).ToList(),
                TenantId = tenant.Id,
                TenantName = tenant.Name
            };

            _cache.Set(cacheKey, userInfo, TimeSpan.FromMinutes(10));

            return userInfo;
        }

        public Task InvalidateUserProfileCacheAsync()
        {
            var identifier = _user.Identifier;
            if (!string.IsNullOrEmpty(identifier))
            {
                var cacheKey = $"user_profile:{identifier}";
                _cache.Remove(cacheKey);
            }
            return Task.CompletedTask;
        }

        public Task InvalidateUserProfileCacheAsync(string identifier)
        {
            var cacheKey = $"user_profile:{identifier}";
            _cache.Remove(cacheKey);
            return Task.CompletedTask;
        }

    }
}
