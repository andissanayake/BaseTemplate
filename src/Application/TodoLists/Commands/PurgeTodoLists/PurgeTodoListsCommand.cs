using BaseTemplate.Domain.Constants;

namespace BaseTemplate.Application.TodoLists.Commands.PurgeTodoLists;

[Authorize(Roles = Roles.Administrator)]
public record PurgeTodoListsCommand : IRequest<bool>;
