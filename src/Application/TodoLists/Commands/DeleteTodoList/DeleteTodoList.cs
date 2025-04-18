using BaseTemplate.Application.Common.Interfaces;
using BaseTemplate.Domain.Entities;

namespace BaseTemplate.Application.TodoLists.Commands.DeleteTodoList;

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
        using var uow = _factory.CreateUOW();
        var entity = await uow.GetAsync<TodoList>(request.Id);

        Guard.Against.NotFound(request.Id, entity);
        await uow.DeleteAsync(entity);
        uow.Commit();
        return Result<bool>.Success(true);
    }
}
