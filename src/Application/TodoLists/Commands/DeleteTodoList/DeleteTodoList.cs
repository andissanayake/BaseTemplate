using BaseTemplate.Application.Common.Interfaces;
using BaseTemplate.Domain.Entities;

namespace BaseTemplate.Application.TodoLists.Commands.DeleteTodoList;

public record DeleteTodoListCommand(int Id) : IRequest;

public class DeleteTodoListCommandHandler : IRequestHandler<DeleteTodoListCommand>
{
    private readonly IUnitOfWorkFactory _factory;

    public DeleteTodoListCommandHandler(IUnitOfWorkFactory factory)
    {
        _factory = factory;
    }

    public async Task Handle(DeleteTodoListCommand request, CancellationToken cancellationToken)
    {
        using var uow = _factory.CreateUOW();
        var entity = await uow.GetAsync<TodoItem>(request.Id);

        Guard.Against.NotFound(request.Id, entity);
        await uow.DeleteAsync(entity);
        uow.Commit();
    }
}
