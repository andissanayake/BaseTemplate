using BaseTemplate.Domain.Constants;

namespace BaseTemplate.Application.Tenants.Commands.UpdateTenant;

[Authorize(Roles = Roles.TenantOwner)]
public record UpdateTenantCommand : IRequest<bool>
{
    public int Id { get; init; }
    public string Name { get; set; } = string.Empty;
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
