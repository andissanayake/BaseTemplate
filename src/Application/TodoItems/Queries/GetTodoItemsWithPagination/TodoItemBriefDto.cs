using BaseTemplate.Domain.Enums;

namespace BaseTemplate.Application.TodoItems.Queries.GetTodoItemsWithPagination;

public class TodoItemBriefDto
{
    public int Id { get; init; }
    public int ListId { get; init; }
    public string? Title { get; init; }
    public string? Note { get; init; }
    public DateTimeOffset? Reminder { get; set; }
    public PriorityLevel Priority { get; set; }
    public bool Done { get; init; }
}
