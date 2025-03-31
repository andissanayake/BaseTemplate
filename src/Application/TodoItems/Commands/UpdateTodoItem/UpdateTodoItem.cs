using BaseTemplate.Application.Common.Interfaces;
using BaseTemplate.Domain.Entities;
using BaseTemplate.Domain.Enums;

namespace BaseTemplate.Application.TodoItems.Commands.UpdateTodoItem;

public record UpdateTodoItemCommand : IRequest
{
    public int Id { get; init; }
    public string? Title { get; init; }
    public string? Note { get; init; }
    public DateTimeOffset? Reminder { get; set; }
    public PriorityLevel? Priority { get; set; }
}

public class UpdateTodoItemCommandHandler : IRequestHandler<UpdateTodoItemCommand>
{
    private readonly IUnitOfWorkFactory _factory;
    private readonly IDomainEventDispatcher _domainEventDispatcher;
    public UpdateTodoItemCommandHandler(IUnitOfWorkFactory factory, IDomainEventDispatcher domainEventDispatcher)
    {
        _factory = factory;
        _domainEventDispatcher = domainEventDispatcher;
    }

    public async Task Handle(UpdateTodoItemCommand request, CancellationToken cancellationToken)
    {
        using var uow = _factory.CreateUOW();
        var entity = await uow.GetAsync<TodoItem>(request.Id);

        Guard.Against.NotFound(request.Id, entity);

        entity.Title = request.Title;
        entity.Title = request.Title;
        entity.Note = request.Note;
        entity.Reminder = request.Reminder;
        entity.Done = false;
        entity.Priority = request.Priority ?? PriorityLevel.None;

        await uow.UpdateAsync(entity);
        uow.Commit();
    }
}
