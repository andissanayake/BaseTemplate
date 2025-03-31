using BaseTemplate.Application.Common.Interfaces;
using BaseTemplate.Domain.Entities;

namespace BaseTemplate.Application.TodoItems.Commands.UpdateTodoItemStatus;

public record UpdateTodoItemStatusCommand : IRequest
{
    public int Id { get; init; }
    public bool Done { get; init; }
}

public class UpdateTodoItemStatusCommandHandler : IRequestHandler<UpdateTodoItemStatusCommand>
{
    private readonly IUnitOfWorkFactory _factory;
    private readonly IDomainEventDispatcher _domainEventDispatcher;

    public UpdateTodoItemStatusCommandHandler(IUnitOfWorkFactory factory, IDomainEventDispatcher domainEventDispatcher)
    {
        _factory = factory;
        _domainEventDispatcher = domainEventDispatcher;
    }

    public async Task Handle(UpdateTodoItemStatusCommand request, CancellationToken cancellationToken)
    {
        using var uow = _factory.CreateUOW();
        var entity = await uow.GetAsync<TodoItem>(request.Id);

        Guard.Against.NotFound(request.Id, entity);

        entity.Done = request.Done;
        await uow.UpdateAsync(entity);
        uow.Commit();
        await _domainEventDispatcher.DispatchDomainEventsAsync(entity);
    }
}
