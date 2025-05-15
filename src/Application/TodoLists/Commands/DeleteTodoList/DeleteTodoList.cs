namespace BaseTemplate.Application.TodoLists.Commands.DeleteTodoList;

[Authorize]
public record DeleteTodoListCommand(int Id) : IRequest<bool>;

public class DeleteTodoListCommandHandler : IRequestHandler<DeleteTodoListCommand, bool>
{
    private readonly IUnitOfWorkFactory _factory;

    public DeleteTodoListCommandHandler(IUnitOfWorkFactory factory)
    {
        _factory = factory;
    }

    public async Task<Result<bool>> HandleAsync(DeleteTodoListCommand request, CancellationToken cancellationToken)
    {
        using var uow = _factory.Create();
        var entity = await uow.GetAsync<TodoList>(request.Id);

        if (entity is null)
        {
            return Result<bool>.NotFound($"TodoList with id {request.Id} not found.");
        }
        await uow.DeleteAsync(entity);
        return Result<bool>.Success(true);
    }
}
