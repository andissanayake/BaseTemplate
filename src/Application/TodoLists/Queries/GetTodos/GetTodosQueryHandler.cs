using BaseTemplate.Domain.Enums;

namespace BaseTemplate.Application.TodoLists.Queries.GetTodos;

public class GetTodosQueryHandler : IRequestHandler<GetTodosQuery, TodosVm>
{
    private readonly IUnitOfWorkFactory _factory;

    public GetTodosQueryHandler(IUnitOfWorkFactory factory)
    {
        _factory = factory;
    }
    public async Task<Result<TodosVm>> HandleAsync(GetTodosQuery request, CancellationToken cancellationToken)
    {
        using var uow = _factory.Create();
        var todoLists = await uow.GetAllAsync<TodoList>();

        var todoDtoList = todoLists.Select(x => new TodoListDto { Colour = x.Colour, Id = x.Id, Title = x.Title }).ToList();
        var res = new TodosVm
        {
            PriorityLevels = Enum.GetValues(typeof(PriorityLevel))
                .Cast<PriorityLevel>()
                .Select(p => new LookupDto { Id = (int)p, Title = p.ToString() })
                .ToList(),
            Lists = todoDtoList,
        };
        return Result<TodosVm>.Success(res);
    }
} 