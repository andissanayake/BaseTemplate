namespace BaseTemplate.Application.Common.Interfaces
{
    public interface IUserProfileService
    {
        Task<UserProfileDto> GetUserProfileAsync();
        Task InvalidateUserProfileCacheAsync();
        Task InvalidateUserProfileCacheAsync(string identifier);
    }

    public class UserProfileDto
    {
        public required int Id { get; set; }
        public required string Identifier { get; set; } = string.Empty;
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required int TenantId { get; set; }
        public required string TenantName { get; set; }
        public required List<string> Roles { get; set; } = new();
    }
}
