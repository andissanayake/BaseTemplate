namespace BaseTemplate.Application.TodoItems.Commands.UpdateTodoItemStatus;

public class UpdateTodoItemStatusCommandHandler : IRequestHandler<UpdateTodoItemStatusCommand, bool>
{
    private readonly IUnitOfWorkFactory _factory;

    public UpdateTodoItemStatusCommandHandler(IUnitOfWorkFactory factory, IIdentityService identityService)
    {
        _factory = factory;
    }

    public async Task<Result<bool>> HandleAsync(UpdateTodoItemStatusCommand request, CancellationToken cancellationToken)
    {
        using var uow = _factory.Create();
        var entity = await uow.GetAsync<TodoItem>(request.Id);

        if (entity is null)
        {
            return Result<bool>.NotFound($"TodoItem with id {request.Id} not found.");
        }

        entity.Done = request.Done;
        await uow.UpdateAsync(entity);
        return Result<bool>.Success(true);
    }
} 