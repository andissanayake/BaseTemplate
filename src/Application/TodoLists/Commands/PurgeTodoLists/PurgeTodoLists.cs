using BaseTemplate.Application.Common.Interfaces;
using BaseTemplate.Domain.Entities;

namespace BaseTemplate.Application.TodoLists.Commands.PurgeTodoLists;

//[Authorize(Roles = Roles.Administrator)]
//[Authorize(Policy = Policies.CanPurge)]
public record PurgeTodoListsCommand : IRequest;

public class PurgeTodoListsCommandHandler : IRequestHandler<PurgeTodoListsCommand>
{
    private readonly IUnitOfWorkFactory _factory;

    public PurgeTodoListsCommandHandler(IUnitOfWorkFactory factory)
    {
        _factory = factory;
    }

    public async Task Handle(PurgeTodoListsCommand request, CancellationToken cancellationToken)
    {
        using var uow = _factory.CreateUOW();
        var items = uow.QueryAsync<TodoList>("");

        await uow.DeleteAsync(items);

        uow.Commit();
    }
}
