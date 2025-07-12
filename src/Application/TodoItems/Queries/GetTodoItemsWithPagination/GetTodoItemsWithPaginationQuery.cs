using System.ComponentModel.DataAnnotations;

namespace BaseTemplate.Application.TodoItems.Queries.GetTodoItemsWithPagination;

[Authorize]
public record GetTodoItemsWithPaginationQuery : IRequest<PaginatedList<TodoItemBriefDto>>
{
    public required int ListId { get; init; }

    [Range(1, int.MaxValue, ErrorMessage = "Page number must be greater than or equal to 1.")]
    public int PageNumber { get; init; } = 1;

    [Range(1, int.MaxValue, ErrorMessage = "Page number must be greater than or equal to 1.")]
    public int PageSize { get; init; } = 10;
} 