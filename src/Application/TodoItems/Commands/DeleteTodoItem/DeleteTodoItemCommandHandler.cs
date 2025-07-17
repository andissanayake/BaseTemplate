namespace BaseTemplate.Application.TodoItems.Commands.DeleteTodoItem;

public class DeleteTodoItemCommandHandler : IRequestHandler<DeleteTodoItemCommand, bool>
{
    private readonly IUnitOfWorkFactory _factory;
    public DeleteTodoItemCommandHandler(IUnitOfWorkFactory factory)
    {
        _factory = factory;
    }

    public async Task<Result<bool>> HandleAsync(DeleteTodoItemCommand request, CancellationToken cancellationToken)
    {
        using var uow = _factory.Create();
        var entity = await uow.GetAsync<TodoItem>(request.Id);

        if (entity is null)
        {
            return Result<bool>.NotFound($"TodoItem with id {request.Id} not found.");
        }

        await uow.DeleteAsync(entity);
        return Result<bool>.Success(true);
    }
} 