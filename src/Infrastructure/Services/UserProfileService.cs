using BaseTemplate.Application.Common.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace BaseTemplate.Infrastructure.Services
{
    public class UserProfileService(IBaseDbContext context, IMemoryCache cache, IUser user) : IUserProfileService
    {
        private readonly IBaseDbContext _context = context;
        private readonly IMemoryCache _cache = cache;
        private readonly IUser _user = user;
        private UserProfile? _userProfile;
        public UserProfile UserProfile
        {
            get
            {
                _userProfile ??= GetUserProfile();
                return _userProfile;
            }
        }

        private UserProfile GetUserProfile()
        {
            var cacheKey = $"user_profile:{_user.Identifier}";
            if (_cache.TryGetValue(cacheKey, out UserProfile? cachedProfile))
            {
                if (cachedProfile != null) return cachedProfile;
            }

            var appUser = _context.AppUser.Single(u => u.SsoId == _user.Identifier);
            var roles = _context.UserRole
                .Where(r => r.UserId == appUser.Id)
                .Select(r => r.Role)
                .ToList();
            if (appUser.TenantId.HasValue)
            {
                var userInfo = new UserProfile()
                {
                    Email = appUser.Email!,
                    Id = appUser.Id,
                    Identifier = appUser.SsoId,
                    Name = appUser.Name!,
                    Roles = roles,
                    TenantId = appUser.TenantId.Value,
                };

                _cache.Set(cacheKey, userInfo, TimeSpan.FromMinutes(10));

                return userInfo;
            }
            else
            {
                throw new InvalidOperationException("There should be a tenant id to access user profile service.");
            }

        }

        public Task InvalidateUserProfileCacheAsync()
        {
            var identifier = _user.Identifier;
            if (!string.IsNullOrEmpty(identifier))
            {
                var cacheKey = $"user_profile:{identifier}";
                _cache.Remove(cacheKey);
                _userProfile = null;
            }
            return Task.CompletedTask;
        }

        public Task InvalidateUserProfileCacheAsync(string identifier)
        {
            var cacheKey = $"user_profile:{identifier}";
            _cache.Remove(cacheKey);

            if (_user.Identifier == identifier)
            {
                _userProfile = null;
            }

            return Task.CompletedTask;
        }
    }
}
