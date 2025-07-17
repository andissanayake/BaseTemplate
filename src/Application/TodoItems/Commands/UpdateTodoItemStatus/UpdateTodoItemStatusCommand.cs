namespace BaseTemplate.Application.TodoItems.Commands.UpdateTodoItemStatus;

[Authorize]
public record UpdateTodoItemStatusCommand : IRequest<bool>
{
    public int Id { get; init; }
    public bool Done { get; init; }
} 