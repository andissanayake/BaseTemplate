using BaseTemplate.Domain.Constants;

namespace BaseTemplate.Application.Tenants.Commands.RequestStaff;

[Authorize(Roles = Roles.TenantOwner)]
public record RequestStaffCommand(int TenantId) : BaseTenantRequest<bool>
{
    public string StaffEmail { get; set; } = string.Empty;
    public string StaffName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}

public class RequestStaffCommandHandler : IRequestHandler<RequestStaffCommand, bool>
{
    private readonly IUnitOfWorkFactory _factory;
    private readonly IUser _user;
    
    public RequestStaffCommandHandler(IUnitOfWorkFactory factory, IUser user)
    {
        _factory = factory;
        _user = user;
    }
    
    public async Task<Result<bool>> HandleAsync(RequestStaffCommand request, CancellationToken cancellationToken)
    {
        using var uow = _factory.Create();
        
        // Verify the tenant exists and the current user is the owner
        var tenant = await uow.GetAsync<Tenant>(request.TenantId);
        if (tenant == null)
        {
            return Result<bool>.NotFound($"Tenant with id {request.TenantId} not found.");
        }
        
        if (tenant.OwnerSsoId != _user.Identifier)
        {
            return Result<bool>.Forbidden("Only tenant owners can request staff members.");
        }
        
        // Check if the staff member already exists
        var existingUser = await uow.QueryFirstOrDefaultAsync<AppUser>(
            "SELECT * FROM app_user WHERE email = @Email", 
            new { Email = request.StaffEmail });
            
        if (existingUser != null)
        {
            // If user exists, check if they're already in this tenant
            if (existingUser.TenantId == request.TenantId)
            {
                return Result<bool>.BadRequest("Staff member is already part of this tenant.");
            }
            
            // If user exists but in different tenant, update their tenant
            existingUser.TenantId = request.TenantId;
            await uow.UpdateAsync(existingUser);
        }
        else
        {
            // Create new user
            var newUser = new AppUser
            {
                SsoId = Guid.NewGuid().ToString(), // Generate temporary SSO ID
                Name = request.StaffName,
                Email = request.StaffEmail,
                TenantId = request.TenantId
            };
            await uow.InsertAsync(newUser);
        }
        
        // Add role for the staff member
        var userRole = new UserRole
        {
            UserSsoId = existingUser?.SsoId ?? Guid.NewGuid().ToString(),
            Role = request.Role
        };
        await uow.InsertAsync(userRole);
        
        return Result<bool>.Success(true, "Staff member request processed successfully.");
    }
} 