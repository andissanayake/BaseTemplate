namespace BaseTemplate.Application.Common.Interfaces
{
    public interface IUserProfileService
    {
        UserProfile UserProfile { get; }
        Task InvalidateUserProfileCacheAsync();
        Task InvalidateUserProfileCacheAsync(string identifier);
    }

    public class UserProfile
    {
        public required int Id { get; set; }
        public required string Identifier { get; set; } = string.Empty;
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required int TenantId { get; set; }
        public required List<string> Roles { get; set; } = new();
    }
}
