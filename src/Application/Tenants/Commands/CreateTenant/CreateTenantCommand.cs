using BaseTemplate.Application.Common.Interfaces;
using BaseTemplate.Application.Common.Models;
using BaseTemplate.Application.Common.RequestHandler;
using BaseTemplate.Application.Common.Security;
using BaseTemplate.Domain.Entities;

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
    public CreateTenantCommandHandler(IUnitOfWorkFactory factory)
    {
        _factory = factory;
    }

    public async Task<Result<int>> HandleAsync(CreateTenantCommand request, CancellationToken cancellationToken)
    {
        using var uow = _factory.Create();
        var entity = new Tenant
        {
            Name = request.Name,
            Address = request.Address,
        };
        await uow.InsertAsync(entity);
        return Result<int>.Success(entity.Id);
    }
}
