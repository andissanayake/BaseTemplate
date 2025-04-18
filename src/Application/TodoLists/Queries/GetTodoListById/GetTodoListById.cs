using BaseTemplate.Application.Common.Interfaces;
using BaseTemplate.Application.TodoLists.Queries;
using BaseTemplate.Domain.Entities;

namespace BaseTemplate.Application.TodoLists.Commands.GetTodoListById;

public record GetTodoListByIdQuery(int Id) : IRequest<TodoListDto>;

public class GetTodoListByIdQueryHandler : IRequestHandler<GetTodoListByIdQuery, TodoListDto>
{
    private readonly IUnitOfWorkFactory _factory;
    public GetTodoListByIdQueryHandler(IUnitOfWorkFactory factory)
    {
        _factory = factory;
    }

    public async Task<Result<TodoListDto>> HandleAsync(GetTodoListByIdQuery request, CancellationToken cancellationToken)
    {
        using var uow = _factory.CreateUOW();
        var entity = await uow.GetAsync<TodoList>(request.Id);
        var todoDto = new TodoListDto() { Colour = entity.Colour, Id = entity.Id, Title = entity.Title };
        return Result<TodoListDto>.Success(todoDto);
    }
}
