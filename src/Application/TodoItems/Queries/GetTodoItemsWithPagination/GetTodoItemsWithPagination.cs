using BaseTemplate.Application.Common.Interfaces;
using BaseTemplate.Application.Common.Models;

namespace BaseTemplate.Application.TodoItems.Queries.GetTodoItemsWithPagination;

public record GetTodoItemsWithPaginationQuery : IRequest<PaginatedList<TodoItemBriefDto>>
{
    public int ListId { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class GetTodoItemsWithPaginationQueryHandler : IRequestHandler<GetTodoItemsWithPaginationQuery, PaginatedList<TodoItemBriefDto>>
{
    private readonly IUnitOfWorkFactory _factory;
    private readonly IMapper _mapper;

    public GetTodoItemsWithPaginationQueryHandler(IUnitOfWorkFactory factory, IMapper mapper)
    {
        _factory = factory;
        _mapper = mapper;
    }

    public async Task<PaginatedList<TodoItemBriefDto>> Handle(GetTodoItemsWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var uow = _factory.CreateUOW();
        //var data = uow.GetAllAsync<TodoItem>();
        //return await _context.TodoItems
        //    .Where(x => x.ListId == request.ListId)
        //    .OrderBy(x => x.Title)
        //    .ProjectTo<TodoItemBriefDto>(_mapper.ConfigurationProvider)
        //    .PaginatedListAsync(request.PageNumber, request.PageSize);
        throw new Exception("Not implemented");
    }
}
