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

    public GetTodoItemsWithPaginationQueryHandler(IUnitOfWorkFactory factory)
    {
        _factory = factory;
    }

    public async Task<Result<PaginatedList<TodoItemBriefDto>>> ValidateAsync(GetTodoItemsWithPaginationQuery request, CancellationToken cancellationToken)
    {
        /*
         *         RuleFor(x => x.ListId)
            .NotEmpty().WithMessage("ListId is required.");

        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1).WithMessage("PageNumber at least greater than or equal to 1.");

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1).WithMessage("PageSize at least greater than or equal to 1.");
         */
        return Result<PaginatedList<TodoItemBriefDto>>.Success(new PaginatedList<TodoItemBriefDto>(new List<TodoItemBriefDto>(), 0, request.PageNumber, request.PageSize));
    }
    public async Task<Result<PaginatedList<TodoItemBriefDto>>> HandleAsync(GetTodoItemsWithPaginationQuery request, CancellationToken cancellationToken)
    {
        using var uow = _factory.CreateUOW();
        var offset = (request.PageNumber - 1) * request.PageSize;

        var countSql = "SELECT COUNT(1) FROM TodoItem WHERE ListId = @ListId";
        var totalCount = await uow.QueryFirstOrDefaultAsync<int>(countSql, new { request.ListId });

        var dataSql = @"
        SELECT Id, ListId, Title, Done, Priority, Reminder, Note
        FROM TodoItem
        WHERE ListId = @ListId
        ORDER BY Title
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
