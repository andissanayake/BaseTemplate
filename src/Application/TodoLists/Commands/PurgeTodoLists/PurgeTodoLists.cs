using BaseTemplate.Application.Common.Interfaces;
using BaseTemplate.Domain.Constants;
using BaseTemplate.Domain.Entities;

namespace BaseTemplate.Application.TodoLists.Commands.PurgeTodoLists;


public record PurgeTodoListsCommand : IRequest<bool>;

public class PurgeTodoListsCommandHandler : IRequestHandler<PurgeTodoListsCommand, bool>
{
    private readonly IUnitOfWorkFactory _factory;
    private readonly IIdentityService _identityService;

    public PurgeTodoListsCommandHandler(IUnitOfWorkFactory factory, IIdentityService identityService)
    {
        _factory = factory;
        _identityService = identityService;
    }
    public async Task<Result<bool>> AuthorizeAsync(PurgeTodoListsCommand request, CancellationToken cancellationToken)
    {
        if (!await _identityService.IsInRoleAsync(Roles.Administrator))
        {
            return Result<bool>.Forbidden("You are not authorized to perform this action.");
        }
        else if (!await _identityService.IsInRoleAsync(Policies.CanPurge))
        {
            return Result<bool>.Forbidden("You are not authorized to perform this action.");
        }
        else
        {
            return Result<bool>.Success(true);
        }
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
