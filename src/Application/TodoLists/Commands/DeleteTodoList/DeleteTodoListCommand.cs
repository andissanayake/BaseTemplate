namespace BaseTemplate.Application.TodoLists.Commands.DeleteTodoList;

[Authorize]
public record DeleteTodoListCommand(int Id) : IRequest<bool>; 