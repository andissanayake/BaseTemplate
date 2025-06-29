using BaseTemplate.Domain.Constants;

namespace BaseTemplate.Application.Tenants.Commands.CreateTenant;

[Authorize]
public record CreateTenantCommand : IRequest<int>
{
    public string Name { get; set; } = string.Empty;
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
        
        // Check if user already has a tenant
        var existingUser = await uow.QueryFirstOrDefaultAsync<AppUser>(
            "SELECT * FROM app_user WHERE sso_id = @Identifier", 
            new { _user.Identifier });
            
        if (existingUser?.TenantId != null)
        {
            return Result<int>.Success(existingUser.TenantId.Value, "User already has a tenant.");
        }

        // Create new tenant
        var tenant = new Tenant
        {
            Name = request.Name,
            Address = request.Address,
            OwnerSsoId = _user.Identifier! // Keep for backward compatibility
        };
        
        var tenantId = await uow.InsertAsync(tenant);
        
        // Update existing user's tenant_id
        if (existingUser != null)
        {
            existingUser.TenantId = tenantId;
            await uow.UpdateAsync(existingUser);
        }
        
        // Add TenantOwner role to the user
        var userRole = new UserRole
        {
            UserSsoId = _user.Identifier!,
            Role = Roles.TenantOwner
        };
        await uow.InsertAsync(userRole);
        
        return Result<int>.Success(tenantId);
    }
}
