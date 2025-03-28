using BaseTemplate.Application.Common.Interfaces;
using BaseTemplate.Application.Common.Models;
using BaseTemplate.Domain.Entities;
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
        var todoLists = await uow.GetAllAsync<TodoList>();
        var todoItems = await uow.GetAllAsync<TodoItem>();

        var todoDtoList = _mapper.Map<List<TodoListDto>>(todoLists);
        var todoDtoItems = _mapper.Map<List<TodoItemDto>>(todoItems);

        var lists = todoDtoList.Select(list => new TodoListDto
        {
            Id = list.Id,
            Title = list.Title,
            Items = todoDtoItems.Where(i => i.ListId == list.Id).ToList()
        });


        return new TodosVm
        {
            PriorityLevels = Enum.GetValues(typeof(PriorityLevel))
                .Cast<PriorityLevel>()
                .Select(p => new LookupDto { Id = (int)p, Title = p.ToString() })
                .ToList(),
            Lists = lists.ToList()

        };
    }
}
