using BaseTemplate.Application.Common.Interfaces;
using BaseTemplate.Application.Common.Models;
using BaseTemplate.Application.Common.RequestHandler;
using BaseTemplate.Application.Common.Security;
using BaseTemplate.Application.TodoLists.Queries;
using BaseTemplate.Domain.Entities;

namespace BaseTemplate.Application.TodoLists.Commands.GetTodoListById;

[Authorize]
public record GetTodoListByIdQuery(int Id) : IRequest<TodoListDto>;

public class GetTodoListByIdQueryHandler : IRequestHandler<GetTodoListByIdQuery, TodoListDto>
{
    private readonly IUnitOfWorkFactory _factory;
    public GetTodoListByIdQueryHandler(IUnitOfWorkFactory factory, IIdentityService identityService)
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
