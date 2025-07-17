using System.ComponentModel.DataAnnotations;
using BaseTemplate.Domain.Enums;

namespace BaseTemplate.Application.TodoItems.Commands.CreateTodoItem;

[Authorize]
public record CreateTodoItemCommand : IRequest<int>
{
    public int ListId { get; init; }

    [Required(ErrorMessage = "Title is required.")]
    [MaxLength(200, ErrorMessage = "The title cannot exceed 200 characters.")]
    [MinLength(1, ErrorMessage = "Title must be at least 1 character long.")]
    public string? Title { get; init; }
    public string? Note { get; init; }
    public DateTimeOffset? Reminder { get; set; }
    public PriorityLevel? Priority { get; set; }
} 