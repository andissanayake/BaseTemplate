using System.ComponentModel.DataAnnotations;
using BaseTemplate.Domain.Constants;

namespace BaseTemplate.Application.Tenants.Commands.CreateTenant;

/// <summary>
/// Command to create a new tenant for the currently authenticated user.
/// </summary>
/// <remarks>
/// <p>This command creates a new tenant and associates the current user as the owner. The workflow includes:</p>
/// <ul>
///   <li><b>Tenant Existence Check:</b> If the user already has a tenant, returns the existing tenant ID and does not create a new tenant.</li>
///   <li><b>Tenant Creation:</b> If the user does not have a tenant, creates a new tenant with the provided name and address, and sets the current user as the owner.</li>
///   <li><b>User Update:</b> Updates the user's tenant association in the database.</li>
///   <li><b>Role Assignment:</b> Adds the <c>TenantOwner</c> role to the user.</li>
///   <li><b>Cache Invalidation:</b> Invalidates the user profile cache to ensure fresh data on subsequent requests.</li>
/// </ul>
/// <p>This command is typically used during onboarding or when a user needs to establish a new tenant context.</p>
/// </remarks>
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

/// <summary>
/// Handles the <see cref="CreateTenantCommand"/> to create a new tenant and assign the current user as the owner.
/// </summary>
/// <remarks>
/// <p>Performs the following steps:</p>
/// <ul>
///   <li>Fetches the current user's profile.</li>
///   <li>If the user already has a tenant, returns the existing tenant ID.</li>
///   <li>Creates a new tenant and associates it with the user.</li>
///   <li>Updates the user's tenant ID and assigns the <c>TenantOwner</c> role.</li>
///   <li>Invalidates the user profile cache to reflect the changes immediately.</li>
/// </ul>
/// <p>Returns a <see cref="Result{int}"/> with the tenant ID or an error message.</p>
/// </remarks>
public class CreateTenantCommandHandler : IRequestHandler<CreateTenantCommand, int>
{
    private readonly IUnitOfWorkFactory _factory;
    private readonly IUser _user;
    private readonly IUserProfileService _userProfileService;
    public CreateTenantCommandHandler(IUnitOfWorkFactory factory, IUser user, IUserProfileService userProfileService)
    {
        _factory = factory;
        _user = user;
        _userProfileService = userProfileService;
    }

    public async Task<Result<int>> HandleAsync(CreateTenantCommand request, CancellationToken cancellationToken)
    {
        using var uow = _factory.Create();
        var transaction = uow.BeginTransaction();
        try
        {
            // Fetch user profile
            var userProfile = await _userProfileService.GetUserProfileByIdentifierAsync(_user.Identifier!);
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
                OwnerSsoId = _user.Identifier!
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
            await _userProfileService.InvalidateUserProfileCacheAsync(_user.Identifier!);

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
