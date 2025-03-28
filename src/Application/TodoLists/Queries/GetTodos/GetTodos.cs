using BaseTemplate.Application.Common.Interfaces;
using BaseTemplate.Application.Common.Models;
using BaseTemplate.Domain.Enums;

namespace BaseTemplate.Application.TodoLists.Queries.GetTodos;

//[Authorize]
public record GetTodosQuery : IRequest<TodosVm>;

public class GetTodosQueryHandler : IRequestHandler<GetTodosQuery, TodosVm>
{
    private readonly IUnitOfWorkFactory _factory;
    private readonly IMapper _mapper;

    public GetTodosQueryHandler(IUnitOfWorkFactory factory, IMapper mapper)
    {
        _factory = factory;
        _mapper = mapper;
    }

    public async Task<TodosVm> Handle(GetTodosQuery request, CancellationToken cancellationToken)
    {
        using var uow = _factory.CreateUOW();
        var sql = @"
            SELECT Id, Title, Colour
            FROM TodoList
            ORDER BY Title";

        var lists = await uow.QueryAsync<TodoListDto>(sql);

        return new TodosVm
        {
            PriorityLevels = Enum.GetValues(typeof(PriorityLevel))
                .Cast<PriorityLevel>()
                .Select(p => new LookupDto { Id = (int)p, Title = p.ToString() })
                .ToList(),
            Lists = (IReadOnlyCollection<TodoListDto>)lists

        };
    }
}
