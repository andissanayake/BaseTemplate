using BaseTemplate.Application.Common.Interfaces;
using BaseTemplate.Domain.Entities;
using BaseTemplate.Domain.Events;

namespace BaseTemplate.Application.TodoItems.Commands.DeleteTodoItem;

public record DeleteTodoItemCommand(int Id) : IRequest<bool>;

public class DeleteTodoItemCommandHandler : IRequestHandler<DeleteTodoItemCommand, bool>
{
    private readonly IUnitOfWorkFactory _factory;
    private readonly IDomainEventDispatcher _domainEventDispatcher;
    public DeleteTodoItemCommandHandler(IUnitOfWorkFactory factory, IDomainEventDispatcher domainEventDispatcher)
    {
        _factory = factory;
        _domainEventDispatcher = domainEventDispatcher;
    }

    public async Task<Result<bool>> HandleAsync(DeleteTodoItemCommand request, CancellationToken cancellationToken)
    {
        using var uow = _factory.CreateUOW();
        var entity = await uow.GetAsync<TodoItem>(request.Id);

        Guard.Against.NotFound(request.Id, entity);

        await uow.DeleteAsync(entity);

        entity.AddDomainEvent(new TodoItemDeletedEvent(entity));

        uow.Commit();
        await _domainEventDispatcher.DispatchDomainEventsAsync(entity);
        return Result<bool>.Success(true);
    }

}
