using BaseTemplate.Application.Common.Interfaces;
using BaseTemplate.Application.Common.Models;
using BaseTemplate.Application.Common.RequestHandler;
using BaseTemplate.Application.Common.Security;
using BaseTemplate.Domain.Constants;
using BaseTemplate.Domain.Entities;

namespace BaseTemplate.Application.TodoLists.Commands.PurgeTodoLists;

[Authorize(Roles = Roles.Administrator, Policy = Policies.CanPurge)]
public record PurgeTodoListsCommand : IRequest<bool>;

public class PurgeTodoListsCommandHandler : BaseRequestHandler<PurgeTodoListsCommand, bool>
{
    private readonly IUnitOfWorkFactory _factory;
    private readonly IIdentityService _identityService;


    public PurgeTodoListsCommandHandler(IUnitOfWorkFactory factory, IIdentityService identityService) : base(identityService)
    {
        _factory = factory;
        _identityService = identityService;
    }
    public override async Task<Result<bool>> HandleAsync(PurgeTodoListsCommand request, CancellationToken cancellationToken)
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
