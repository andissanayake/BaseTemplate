using System.ComponentModel.DataAnnotations;
using BaseTemplate.Domain.Constants;

namespace BaseTemplate.Application.Tenants.Commands.UpdateTenant;

[Authorize(Roles = Roles.TenantManager + "," + Roles.TenantOwner)]
public record UpdateTenantCommand : IRequest<bool>
{
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
    private readonly IUserProfileService _userProfileService;
    public UpdateTenantCommandHandler(IUnitOfWorkFactory factory, IUserProfileService userProfileService)
    {
        _factory = factory;
        _userProfileService = userProfileService;
    }
    public async Task<Result<bool>> HandleAsync(UpdateTenantCommand request, CancellationToken cancellationToken)
    {
        var userProfile = await _userProfileService.GetUserProfileAsync();
        if (userProfile == null || userProfile.TenantId == null)
        {
            return Result<bool>.Failure("No tenant context found for current user.");
        }
        var tenantId = userProfile.TenantId;
        using var uow = _factory.Create();
        var entity = await uow.GetAsync<Tenant>(tenantId);

        if (entity is null)
        {
            return Result<bool>.NotFound($"Tenant with id {tenantId} not found.");
        }
        entity.Name = request.Name;
        entity.Address = request.Address;
        await uow.UpdateAsync(entity);
        return Result<bool>.Success(true);
    }
}
