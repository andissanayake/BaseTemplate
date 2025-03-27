using BaseTemplate.Application.Common.Interfaces;
using BaseTemplate.Domain.Entities;
using BaseTemplate.Domain.Events;

namespace BaseTemplate.Application.TodoItems.Commands.DeleteTodoItem;

public record DeleteTodoItemCommand(int Id) : IRequest;

public class DeleteTodoItemCommandHandler : IRequestHandler<DeleteTodoItemCommand>
{
    private readonly IUnitOfWorkFactory _factory;

    public DeleteTodoItemCommandHandler(IUnitOfWorkFactory factory)
    {
        _factory = factory;
    }

    public async Task Handle(DeleteTodoItemCommand request, CancellationToken cancellationToken)
    {
        var uow = _factory.CreateUOW();
        var entity = await uow.GetAsync<TodoItem>(request.Id);

        Guard.Against.NotFound(request.Id, entity);

        await uow.DeleteAsync(entity);

        entity.AddDomainEvent(new TodoItemDeletedEvent(entity));

        uow.Commit();
    }

}
