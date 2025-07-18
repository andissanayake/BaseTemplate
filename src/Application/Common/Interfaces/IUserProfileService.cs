namespace BaseTemplate.Application.Common.Interfaces
{
    public interface IUserProfileService
    {
        Task<UserProfileDto?> GetUserProfileByIdentifierAsync(string identifier);
    }

    public class UserProfileDto
    {
        public int Id { get; set; }
        public string Identifier { get; set; } = string.Empty;
        public string? Name { get; set; }
        public string? Email { get; set; }
        public int? TenantId { get; set; }
        public string? TenantName { get; set; }
        public List<string> Roles { get; set; } = new();
    }
}
