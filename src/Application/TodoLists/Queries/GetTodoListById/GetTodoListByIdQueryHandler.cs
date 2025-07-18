using BaseTemplate.Application.TodoLists.Queries;

namespace BaseTemplate.Application.TodoLists.Commands.GetTodoListById;

public class GetTodoListByIdQueryHandler : IRequestHandler<GetTodoListByIdQuery, TodoListDto>
{
    private readonly IUnitOfWorkFactory _factory;
    public GetTodoListByIdQueryHandler(IUnitOfWorkFactory factory)
    {
        _factory = factory;
    }

    public async Task<Result<TodoListDto>> HandleAsync(GetTodoListByIdQuery request, CancellationToken cancellationToken)
    {
        using var uow = _factory.Create();
        var entity = await uow.GetAsync<TodoList>(request.Id);
        if (entity is null)
        {
            return Result<TodoListDto>.NotFound($"TodoList with id {request.Id} not found.");
        }
        var todoDto = new TodoListDto() { Colour = entity.Colour, Id = entity.Id, Title = entity.Title };
        return Result<TodoListDto>.Success(todoDto);
    }
}
