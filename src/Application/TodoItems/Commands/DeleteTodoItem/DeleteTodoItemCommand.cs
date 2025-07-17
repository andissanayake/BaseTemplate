namespace BaseTemplate.Application.TodoItems.Commands.DeleteTodoItem;

[Authorize]
public record DeleteTodoItemCommand(int Id) : IRequest<bool>; 