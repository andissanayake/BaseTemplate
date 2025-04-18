using BaseTemplate.Application.Common.Interfaces;
using BaseTemplate.Domain.Entities;

namespace BaseTemplate.Application.TodoLists.Commands.PurgeTodoLists;


public record PurgeTodoListsCommand : IRequest<bool>;

public class PurgeTodoListsCommandHandler : IRequestHandler<PurgeTodoListsCommand, bool>
{
    private readonly IUnitOfWorkFactory _factory;

    public PurgeTodoListsCommandHandler(IUnitOfWorkFactory factory)
    {
        _factory = factory;
    }
    public async Task<Result<bool>> AuthorizeAsync(PurgeTodoListsCommand request, CancellationToken cancellationToken)
    {
        /*
         //[Authorize(Roles = Roles.Administrator)]
//[Authorize(Policy = Policies.CanPurge)]
         */
        return Result<bool>.Success(true);
    }
    public async Task<Result<bool>> HandleAsync(PurgeTodoListsCommand request, CancellationToken cancellationToken)
    {
        using var uow = _factory.CreateUOW();
        var items = await uow.GetAllAsync<TodoList>();
        var items2 = await uow.GetAllAsync<TodoItem>();

        await uow.DeleteAsync(items);
        await uow.DeleteAsync(items2);

        uow.Commit();
        return Result<bool>.Success(true);
    }
}
