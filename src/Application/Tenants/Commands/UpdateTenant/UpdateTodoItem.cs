using BaseTemplate.Application.Common.Interfaces;
using BaseTemplate.Application.Common.Models;
using BaseTemplate.Application.Common.RequestHandler;
using BaseTemplate.Application.Common.Security;
using BaseTemplate.Domain.Entities;

namespace BaseTemplate.Application.Tenants.Commands.UpdateTenant;

[Authorize]
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
