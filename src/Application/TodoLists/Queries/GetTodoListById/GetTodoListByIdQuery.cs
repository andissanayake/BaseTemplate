using BaseTemplate.Application.TodoLists.Queries;

namespace BaseTemplate.Application.TodoLists.Commands.GetTodoListById;

[Authorize]
public record GetTodoListByIdQuery(int Id) : IRequest<TodoListDto>; 