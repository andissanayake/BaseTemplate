using BaseTemplate.Application.Common.Interfaces;
using BaseTemplate.Application.Common.Models;
using BaseTemplate.Application.Common.RequestHandler;
using BaseTemplate.Application.Common.Security;
using BaseTemplate.Domain.Entities;

namespace BaseTemplate.Application.TodoLists.Commands.DeleteTodoList;

[Authorize]
public record DeleteTodoListCommand(int Id) : IRequest<bool>;

public class DeleteTodoListCommandHandler : BaseRequestHandler<DeleteTodoListCommand, bool>
{
    private readonly IUnitOfWorkFactory _factory;

    public DeleteTodoListCommandHandler(IUnitOfWorkFactory factory, IIdentityService identityService) : base(identityService)
    {
        _factory = factory;
    }

    public override async Task<Result<bool>> HandleAsync(DeleteTodoListCommand request, CancellationToken cancellationToken)
    {
        using var uow = _factory.CreateUOW();
        var entity = await uow.GetAsync<TodoList>(request.Id);

        if (entity is null)
        {
            return Result<bool>.NotFound($"TodoList with id {request.Id} not found.");
        }

        await uow.DeleteAsync(entity);
        uow.Commit();
        return Result<bool>.Success(true);
    }
}
