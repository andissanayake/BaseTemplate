using BaseTemplate.Domain.ValueObjects;

namespace BaseTemplate.Application.TodoLists.Commands.CreateTodoList;

public class CreateTodoListCommandHandler : IRequestHandler<CreateTodoListCommand, int>
{
    private readonly IUnitOfWorkFactory _factory;

    public CreateTodoListCommandHandler(IUnitOfWorkFactory factory)
    {
        _factory = factory;
    }

    public async Task<Result<int>> HandleAsync(CreateTodoListCommand request, CancellationToken cancellationToken)
    {
        var entity = new TodoList();
        entity.Title = request.Title;

        if (request.Colour is not null)
            entity.Colour = Colour.From(request.Colour).Code;

        using var uow = _factory.Create();
        await uow.InsertAsync(entity);
        return Result<int>.Success(entity.Id);
    }
}
