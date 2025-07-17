using BaseTemplate.Domain.ValueObjects;

namespace BaseTemplate.Application.TodoLists.Commands.UpdateTodoList;

public class UpdateTodoListCommandHandler : IRequestHandler<UpdateTodoListCommand, bool>
{
    private readonly IUnitOfWorkFactory _factory;

    public UpdateTodoListCommandHandler(IUnitOfWorkFactory factory)
    {
        _factory = factory;
    }

    public async Task<Result<bool>> HandleAsync(UpdateTodoListCommand request, CancellationToken cancellationToken)
    {
        using var uow = _factory.Create();
        var entity = await uow.GetAsync<TodoList>(request.Id);

        if (entity is null)
        {
            return Result<bool>.NotFound($"TodoList with id {request.Id} not found.");
        }

        entity.Title = request.Title;
        entity.Colour = request.Colour ?? Colour.White.Code;
        await uow.UpdateAsync(entity);
        return Result<bool>.Success(true);
    }
} 