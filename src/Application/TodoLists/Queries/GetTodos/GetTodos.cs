﻿using BaseTemplate.Application.Common.Interfaces;
using BaseTemplate.Application.Common.Models;
using BaseTemplate.Application.Common.RequestHandler;
using BaseTemplate.Application.Common.Security;
using BaseTemplate.Domain.Entities;
using BaseTemplate.Domain.Enums;

namespace BaseTemplate.Application.TodoLists.Queries.GetTodos;

[Authorize]
public record GetTodosQuery : IRequest<TodosVm>;

public class GetTodosQueryHandler : BaseRequestHandler<GetTodosQuery, TodosVm>
{
    private readonly IUnitOfWorkFactory _factory;

    public GetTodosQueryHandler(IUnitOfWorkFactory factory, IIdentityService identityService) : base(identityService)
    {
        _factory = factory;
    }
    public override async Task<Result<TodosVm>> HandleAsync(GetTodosQuery request, CancellationToken cancellationToken)
    {
        using var uow = _factory.CreateUOW();
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
