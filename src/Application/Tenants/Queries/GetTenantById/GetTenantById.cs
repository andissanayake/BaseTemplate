﻿using BaseTemplate.Domain.Constants;

namespace BaseTemplate.Application.TenantLists.Commands.GetTenantById;

[Authorize(Roles = Roles.Administrator)]
public record GetTenantByIdQuery(int Id) : IRequest<GetTenantResponse>;

public record GetTenantResponse
{
    public int Id { get; init; }
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }

}

public class GetTenantByIdQueryHandler : IRequestHandler<GetTenantByIdQuery, GetTenantResponse>
{
    private readonly IUnitOfWorkFactory _factory;
    public GetTenantByIdQueryHandler(IUnitOfWorkFactory factory, IIdentityService identityService)
    {
        _factory = factory;
    }

    public async Task<Result<GetTenantResponse>> HandleAsync(GetTenantByIdQuery request, CancellationToken cancellationToken)
    {
        using var uow = _factory.Create();
        var entity = await uow.GetAsync<Tenant>(request.Id);
        if (entity is null)
        {
            return Result<GetTenantResponse>.NotFound($"TenantList with id {request.Id} not found.");
        }
        var tenant = new GetTenantResponse() { Name = entity.Name, Id = entity.Id, Address = entity.Address };
        return Result<GetTenantResponse>.Success(tenant);
    }
}
