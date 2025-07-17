using BaseTemplate.Domain.Constants;

namespace BaseTemplate.Application.TodoLists.Commands.PurgeTodoLists;

[Authorize(Roles = Roles.Administrator, Policy = Policies.CanPurge)]
public record PurgeTodoListsCommand : IRequest<bool>; 