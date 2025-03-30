using BaseTemplate.Application.Common.Interfaces;
using BaseTemplate.Application.TodoLists.Queries;
using BaseTemplate.Domain.Entities;

namespace BaseTemplate.Application.TodoLists.Commands.GetTodoListById;

public record GetTodoListByIdQuery(int Id) : IRequest<TodoListDto>;

public class GetTodoListByIdQueryHandler : IRequestHandler<GetTodoListByIdQuery, TodoListDto>
{
    private readonly IUnitOfWorkFactory _factory;
    private readonly IMapper _mapper;
    public GetTodoListByIdQueryHandler(IUnitOfWorkFactory factory, IMapper mapper)
    {
        _factory = factory;
        _mapper = mapper;
    }

    public async Task<TodoListDto> Handle(GetTodoListByIdQuery request, CancellationToken cancellationToken)
    {
        using var uow = _factory.CreateUOW();
        var entity = await uow.GetAsync<TodoList>(request.Id);
        var todoDto = _mapper.Map<TodoListDto>(entity);
        return todoDto;
    }
}
