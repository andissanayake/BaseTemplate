using System.ComponentModel.DataAnnotations;

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
    private readonly IUser _user;

    public CreateTenantCommandHandler(IUnitOfWorkFactory factory, IUser user)
    {
        _factory = factory;
        _user = user;
    }

    public async Task<Result<int>> HandleAsync(CreateTenantCommand request, CancellationToken cancellationToken)
    {
        using var uow = _factory.Create();
        var transaction = uow.BeginTransaction();
        try
        {
            // Load existing user - user should already exist at this point
            var existingUser = await uow.QuerySingleAsync<AppUser>(
                "SELECT * FROM app_user WHERE sso_id = @SsoId",
                new { SsoId = _user.Identifier });

            // Create new tenant
            var tenant = new Tenant
            {
                Name = request.Name,
                Address = request.Address,
                OwnerSsoId = _user.Identifier
            };
            var tenantId = await uow.InsertAsync(tenant);

            // Update user's tenant_id
            await uow.ExecuteAsync(
                "UPDATE app_user SET tenant_id = @TenantId, last_modified = @LastModified WHERE id = @UserId",
                new { TenantId = tenantId, LastModified = DateTimeOffset.UtcNow, UserId = existingUser.Id });

            // Add TenantOwner role to the user
            var userRole = new UserRole
            {
                UserId = existingUser.Id,
                Role = Roles.TenantOwner
            };
            await uow.InsertAsync(userRole);

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
