namespace BaseTemplate.Application.TodoItems.Queries.GetTodoItemsWithPagination;

public class GetTodoItemsWithPaginationQueryHandler : IRequestHandler<GetTodoItemsWithPaginationQuery, PaginatedList<TodoItemBriefDto>>
{
    private readonly IUnitOfWorkFactory _factory;

    public GetTodoItemsWithPaginationQueryHandler(IUnitOfWorkFactory factory)
    {
        _factory = factory;
    }
    public async Task<Result<PaginatedList<TodoItemBriefDto>>> HandleAsync(GetTodoItemsWithPaginationQuery request, CancellationToken cancellationToken)
    {
        using var uow = _factory.Create();
        var offset = (request.PageNumber - 1) * request.PageSize;

        var countSql = "SELECT COUNT(1) FROM todo_item WHERE list_id = @ListId";
        var totalCount = await uow.QueryFirstOrDefaultAsync<int>(countSql, new { request.ListId });

        var dataSql = @"
        SELECT id, list_id, title, done, priority, reminder, note
        FROM todo_item
        WHERE list_id = @ListId
        ORDER BY title
        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

        var items = await uow.QueryAsync<TodoItemBriefDto>(dataSql, new
        {
            request.ListId,
            Offset = offset,
            request.PageSize
        });

        return Result<PaginatedList<TodoItemBriefDto>>.Success(new PaginatedList<TodoItemBriefDto>(items.ToList(), totalCount, request.PageNumber, request.PageSize));
    }
} 