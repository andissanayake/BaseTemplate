using BaseTemplate.Application.Common.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace BaseTemplate.Infrastructure.Services
{
    public class UserProfileService : IUserProfileService
    {
        private readonly IUnitOfWorkFactory _factory;
        private readonly IMemoryCache _cache;

        public UserProfileService(IUnitOfWorkFactory factory, IMemoryCache cache)
        {
            _factory = factory;
            _cache = cache;
        }

        public async Task<UserProfileDto?> GetUserProfileByIdentifierAsync(string identifier)
        {
            var cacheKey = $"user_profile:{identifier}";
            if (_cache.TryGetValue(cacheKey, out UserProfileDto? cachedProfile))
            {
                return cachedProfile;
            }

            using var uow = _factory.Create();
            var userInfo = await uow.QueryFirstOrDefaultAsync<UserProfileDto>(@"
                SELECT u.Id, u.sso_id, u.Name, u.Email, t.Id as Tenant_Id, t.Name as TenantName
                FROM app_user u
                LEFT JOIN Tenant t ON u.Tenant_Id = t.Id
                WHERE u.sso_id = @Identifier", new { Identifier = identifier });

            if (userInfo == null)
                return null;

            var roles = (await uow.QueryAsync<string>(
                "select role from user_role where user_id = @UserId",
                new { UserId = userInfo.Id })).ToList();
            userInfo.Identifier = identifier;
            userInfo.Roles = roles;

            _cache.Set(cacheKey, userInfo, TimeSpan.FromMinutes(10));

            return userInfo;
        }

        public Task InvalidateUserProfileCacheAsync(string identifier)
        {
            var cacheKey = $"user_profile:{identifier}";
            _cache.Remove(cacheKey);
            return Task.CompletedTask;
        }
    }
}
