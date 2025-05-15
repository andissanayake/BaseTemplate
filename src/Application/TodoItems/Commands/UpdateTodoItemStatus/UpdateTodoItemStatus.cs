using BaseTemplate.Application.Common.Interfaces;
using BaseTemplate.Application.Common.Models;
using BaseTemplate.Application.Common.RequestHandler;
using BaseTemplate.Application.Common.Security;
using BaseTemplate.Domain.Entities;

namespace BaseTemplate.Application.TodoItems.Commands.UpdateTodoItemStatus;

[Authorize]
public record UpdateTodoItemStatusCommand : IRequest<bool>
{
    public int Id { get; init; }
    public bool Done { get; init; }
}

public class UpdateTodoItemStatusCommandHandler : IRequestHandler<UpdateTodoItemStatusCommand, bool>
{
    private readonly IUnitOfWorkFactory _factory;

    public UpdateTodoItemStatusCommandHandler(IUnitOfWorkFactory factory, IIdentityService identityService)
    {
        _factory = factory;
    }

    public async Task<Result<bool>> HandleAsync(UpdateTodoItemStatusCommand request, CancellationToken cancellationToken)
    {
        using var uow = _factory.Create();
        var entity = await uow.GetAsync<TodoItem>(request.Id);

        if (entity is null)
        {
            return Result<bool>.NotFound($"TodoItem with id {request.Id} not found.");
        }

        entity.Done = request.Done;
        await uow.UpdateAsync(entity);
        return Result<bool>.Success(true);
    }
}
