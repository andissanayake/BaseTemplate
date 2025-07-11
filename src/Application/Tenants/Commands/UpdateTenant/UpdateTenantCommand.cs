using System.ComponentModel.DataAnnotations;
using BaseTemplate.Domain.Constants;

namespace BaseTemplate.Application.Tenants.Commands.UpdateTenant;

[Authorize(Roles = Roles.TenantManager + "," + Roles.TenantOwner)]
public record UpdateTenantCommand(int TenantId) : BaseTenantRequest<bool>(TenantId)
{
    public int Id { get; init; }
    
    [Required]
    [MinLength(2, ErrorMessage = "Tenant name must be at least 2 characters long.")]
    [MaxLength(100, ErrorMessage = "Tenant name cannot exceed 100 characters.")]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(500, ErrorMessage = "Address cannot exceed 500 characters.")]
    public string? Address { get; set; }
}

public class UpdateTenantCommandHandler : IRequestHandler<UpdateTenantCommand, bool>
{
    private readonly IUnitOfWorkFactory _factory;
    public UpdateTenantCommandHandler(IUnitOfWorkFactory factory, IIdentityService identityService)
    {
        _factory = factory;
    }
    public async Task<Result<bool>> HandleAsync(UpdateTenantCommand request, CancellationToken cancellationToken)
    {
        using var uow = _factory.Create();
        var entity = await uow.GetAsync<Tenant>(request.Id);

        if (entity is null)
        {
            return Result<bool>.NotFound($"Tenant with id {request.Id} not found.");
        }
        entity.Name = request.Name;
        entity.Address = request.Address;
        await uow.UpdateAsync(entity);
        return Result<bool>.Success(true);
    }
}
