using BaseTemplate.Domain.Enums;

namespace BaseTemplate.Application.TodoItems.Commands.UpdateTodoItem;

public class UpdateTodoItemCommandHandler : IRequestHandler<UpdateTodoItemCommand, bool>
{
    private readonly IUnitOfWorkFactory _factory;
    public UpdateTodoItemCommandHandler(IUnitOfWorkFactory factory)
    {
        _factory = factory;
    }
    public async Task<Result<bool>> HandleAsync(UpdateTodoItemCommand request, CancellationToken cancellationToken)
    {
        using var uow = _factory.Create();
        var entity = await uow.GetAsync<TodoItem>(request.Id);

        if (entity is null)
        {
            return Result<bool>.NotFound($"TodoItem with id {request.Id} not found.");
        }
        entity.Title = request.Title;
        entity.Note = request.Note;
        entity.Reminder = request.Reminder?.UtcDateTime;
        entity.Done = false;
        entity.Priority = request.Priority ?? PriorityLevel.None;
        await uow.UpdateAsync(entity);
        return Result<bool>.Success(true);
    }
}
