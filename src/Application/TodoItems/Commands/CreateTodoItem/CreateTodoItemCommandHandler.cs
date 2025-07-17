using BaseTemplate.Domain.Enums;

namespace BaseTemplate.Application.TodoItems.Commands.CreateTodoItem;

public class CreateTodoItemCommandHandler : IRequestHandler<CreateTodoItemCommand, int>
{
    private readonly IUnitOfWorkFactory _factory;
    public CreateTodoItemCommandHandler(IUnitOfWorkFactory factory)
    {
        _factory = factory;
    }

    public async Task<Result<int>> HandleAsync(CreateTodoItemCommand request, CancellationToken cancellationToken)
    {
        using var uow = _factory.Create();
        var entity = new TodoItem
        {
            ListId = request.ListId,
            Title = request.Title,
            Note = request.Note,
            Reminder = request.Reminder?.UtcDateTime,
            Priority = request.Priority ?? PriorityLevel.None,
            Done = false
        };
        await uow.InsertAsync(entity);
        return Result<int>.Success(entity.Id);
    }
} 