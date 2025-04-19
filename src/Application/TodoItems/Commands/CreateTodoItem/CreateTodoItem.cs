using System.ComponentModel.DataAnnotations;
using BaseTemplate.Application.Common.Interfaces;
using BaseTemplate.Domain.Entities;
using BaseTemplate.Domain.Enums;
using BaseTemplate.Domain.Events;

namespace BaseTemplate.Application.TodoItems.Commands.CreateTodoItem;

public record CreateTodoItemCommand : IRequest<int>
{
    public int ListId { get; init; }

    [MaxLength(200, ErrorMessage = "The title cannot exceed 200 characters.")]
    public string? Title { get; init; }
    public string? Note { get; init; }
    public DateTimeOffset? Reminder { get; set; }
    public PriorityLevel? Priority { get; set; }
}

public class CreateTodoItemCommandHandler : IRequestHandler<CreateTodoItemCommand, int>
{
    private readonly IUnitOfWorkFactory _factory;
    private readonly IDomainEventDispatcher _domainEventDispatcher;
    public CreateTodoItemCommandHandler(IUnitOfWorkFactory factory, IDomainEventDispatcher domainEventDispatcher)
    {
        _factory = factory;
        _domainEventDispatcher = domainEventDispatcher;
    }
    public Result<int> ValidateAsync(CreateTodoItemCommand request, CancellationToken cancellationToken)
    {
        return Result<int>.Success(0);
    }

    public async Task<Result<int>> HandleAsync(CreateTodoItemCommand request, CancellationToken cancellationToken)
    {
        using var uow = _factory.CreateUOW();
        var entity = new TodoItem
        {
            ListId = request.ListId,
            Title = request.Title,
            Note = request.Note,
            Reminder = request.Reminder,
            Priority = request.Priority ?? PriorityLevel.None,
            Done = false
        };
        await uow.InsertAsync(entity);
        entity.AddDomainEvent(new TodoItemCreatedEvent(entity));
        uow.Commit();
        await _domainEventDispatcher.DispatchDomainEventsAsync(entity);
        return Result<int>.Success(entity.Id);
    }
}
