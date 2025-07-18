using System.ComponentModel.DataAnnotations;
using BaseTemplate.Domain.Constants;

namespace BaseTemplate.Application.Tenants.Commands.CreateTenant;

[Authorize]
public record CreateTenantCommand : IRequest<int>
{
    [Required]
    [MinLength(2, ErrorMessage = "Tenant name must be at least 2 characters long.")]
    [MaxLength(100, ErrorMessage = "Tenant name cannot exceed 100 characters.")]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500, ErrorMessage = "Address cannot exceed 500 characters.")]
    public string? Address { get; set; }
}

public class CreateTenantCommandHandler : IRequestHandler<CreateTenantCommand, int>
{
    private readonly IUnitOfWorkFactory _factory;
    private readonly IUserProfileService _userProfileService;
    public CreateTenantCommandHandler(IUnitOfWorkFactory factory, IUserProfileService userProfileService)
    {
        _factory = factory;
        _userProfileService = userProfileService;
    }

    public async Task<Result<int>> HandleAsync(CreateTenantCommand request, CancellationToken cancellationToken)
    {
        using var uow = _factory.Create();
        var transaction = uow.BeginTransaction();
        try
        {
            // Fetch user profile
            var userProfile = await _userProfileService.GetUserProfileAsync();
            if (userProfile == null)
            {
                transaction.Rollback();
                return Result<int>.Failure("User not found.");
            }

            // Check if user already has a tenant
            if (userProfile.TenantId != null)
            {
                transaction.Rollback();
                return Result<int>.Success(userProfile.TenantId.Value, "User already has a tenant.");
            }

            // Create new tenant
            var tenant = new Tenant
            {
                Name = request.Name,
                Address = request.Address,
                OwnerSsoId = userProfile.Identifier!
            };

            var tenantId = await uow.InsertAsync(tenant);

            // Update existing user's tenant_id
            var appUser = new AppUser
            {
                Id = userProfile.Id,
                SsoId = userProfile.Identifier,
                Name = userProfile.Name,
                Email = userProfile.Email,
                TenantId = tenantId
            };
            await uow.UpdateAsync(appUser);

            // Add TenantOwner role to the user
            var userRole = new UserRole
            {
                UserId = userProfile.Id,
                Role = Roles.TenantOwner
            };
            await uow.InsertAsync(userRole);

            // Invalidate user profile cache
            await _userProfileService.InvalidateUserProfileCacheAsync();

            transaction.Commit();
            return Result<int>.Success(tenantId);
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            return Result<int>.Failure($"Failed to create tenant: {ex.Message}");
        }
    }
}
