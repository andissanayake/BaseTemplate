using BaseTemplate.Application.Common.Interfaces;
using BaseTemplate.Application.Common.Models;
using BaseTemplate.Application.Common.RequestHandler;
using BaseTemplate.Application.Common.Security;
using BaseTemplate.Domain.Entities;
using BaseTemplate.Domain.Events;

namespace BaseTemplate.Application.TodoItems.Commands.DeleteTodoItem;

[Authorize]
public record DeleteTodoItemCommand(int Id) : IRequest<bool>;

public class DeleteTodoItemCommandHandler : BaseRequestHandler<DeleteTodoItemCommand, bool>
{
    private readonly IUnitOfWorkFactory _factory;
    private readonly IDomainEventDispatcher _domainEventDispatcher;
    public DeleteTodoItemCommandHandler(IUnitOfWorkFactory factory, IDomainEventDispatcher domainEventDispatcher, IIdentityService identityService) : base(identityService)
    {
        _factory = factory;
        _domainEventDispatcher = domainEventDispatcher;
    }

    public override async Task<Result<bool>> HandleAsync(DeleteTodoItemCommand request, CancellationToken cancellationToken)
    {
        using var uow = _factory.CreateUOW();
        var entity = await uow.GetAsync<TodoItem>(request.Id);

        if (entity is null)
        {
            return Result<bool>.NotFound($"TodoItem with id {request.Id} not found.");
        }

        await uow.DeleteAsync(entity);

        entity.AddDomainEvent(new TodoItemDeletedEvent(entity));

        uow.Commit();
        await _domainEventDispatcher.DispatchDomainEventsAsync(entity);
        return Result<bool>.Success(true);
    }

}
