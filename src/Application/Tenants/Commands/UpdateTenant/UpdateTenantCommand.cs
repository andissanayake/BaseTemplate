using System.ComponentModel.DataAnnotations;

namespace BaseTemplate.Application.Tenants.Commands.UpdateTenant;

[Authorize(Roles = Roles.TenantManager)]
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
    private readonly IUserTenantProfileService _userProfileService;
    public UpdateTenantCommandHandler(IUnitOfWorkFactory factory, IUserTenantProfileService userProfileService)
    {
        _factory = factory;
        _userProfileService = userProfileService;
    }
    public async Task<Result<bool>> HandleAsync(UpdateTenantCommand request, CancellationToken cancellationToken)
    {
        var userProfile = await _userProfileService.GetUserProfileAsync();
        using var uow = _factory.Create();
        var entity = await uow.GetAsync<Tenant>(userProfile.TenantId);

        if (entity is null)
        {
            return Result<bool>.NotFound($"Tenant with id {userProfile.TenantId} not found.");
        }
        entity.Name = request.Name;
        entity.Address = request.Address;
        await _userProfileService.InvalidateUserProfileCacheAsync();
        await uow.UpdateAsync(entity);
        return Result<bool>.Success(true);
    }
}
