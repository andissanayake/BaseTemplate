namespace BaseTemplate.Application.Items.Commands.DeleteItem;

[Authorize]
public record DeleteItemCommand(int TenantId, int Id) : BaseTenantRequest<bool>(TenantId);

public class DeleteItemCommandHandler : IRequestHandler<DeleteItemCommand, bool>
{
    private readonly IUnitOfWorkFactory _factory;

    public DeleteItemCommandHandler(IUnitOfWorkFactory factory)
    {
        _factory = factory;
    }

    public async Task<Result<bool>> HandleAsync(DeleteItemCommand request, CancellationToken cancellationToken)
    {
        using var uow = _factory.Create();
        var entity = await uow.GetAsync<Item>(request.Id);

        if (entity is null)
        {
            return Result<bool>.NotFound($"Item with id {request.Id} not found.");
        }

        // Soft delete by setting IsActive to false
        entity.IsActive = false;
        await uow.UpdateAsync(entity);

        return Result<bool>.Success(true);
    }
}
