﻿using BaseTemplate.Domain.Constants;

namespace BaseTemplate.Application.TodoLists.Commands.PurgeTodoLists;

[Authorize(Roles = Roles.Administrator, Policy = Policies.CanPurge)]
public record PurgeTodoListsCommand : IRequest<bool>;

public class PurgeTodoListsCommandHandler : IRequestHandler<PurgeTodoListsCommand, bool>
{
    private readonly IUnitOfWorkFactory _factory;


    public PurgeTodoListsCommandHandler(IUnitOfWorkFactory factory)
    {
        _factory = factory;
    }
    public async Task<Result<bool>> HandleAsync(PurgeTodoListsCommand request, CancellationToken cancellationToken)
    {
        using var uow = _factory.Create();
        var transaction = uow.BeginTransaction();
        var items = await uow.GetAllAsync<TodoList>();
        var items2 = await uow.GetAllAsync<TodoItem>();

        await uow.DeleteAsync(items);
        await uow.DeleteAsync(items2);
        transaction.Commit();
        return Result<bool>.Success(true);
    }
}
