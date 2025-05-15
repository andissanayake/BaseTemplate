using BaseTemplate.Application.Common.Interfaces;
using BaseTemplate.Application.Common.Models;
using BaseTemplate.Application.Common.RequestHandler;
using BaseTemplate.Application.Common.Security;
using BaseTemplate.Domain.Entities;

namespace BaseTemplate.Application.TenantLists.Commands.GetTenantListById;

[Authorize]
public record GetTenantByIdQuery(int Id) : IRequest<GetTenantResponce>;

public record GetTenantResponce
{
    public int Id { get; init; }
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }

}

public class GetTenantByIdQueryHandler : IRequestHandler<GetTenantByIdQuery, GetTenantResponce>
{
    private readonly IUnitOfWorkFactory _factory;
    public GetTenantByIdQueryHandler(IUnitOfWorkFactory factory, IIdentityService identityService)
    {
        _factory = factory;
    }

    public async Task<Result<GetTenantResponce>> HandleAsync(GetTenantByIdQuery request, CancellationToken cancellationToken)
    {
        using var uow = _factory.Create();
        var entity = await uow.GetAsync<Tenant>(request.Id);
        if (entity is null)
        {
            return Result<GetTenantResponce>.NotFound($"TenantList with id {request.Id} not found.");
        }
        var tenant = new GetTenantResponce() { Name = entity.Name, Id = entity.Id, Address = entity.Address };
        return Result<GetTenantResponce>.Success(tenant);
    }
}
