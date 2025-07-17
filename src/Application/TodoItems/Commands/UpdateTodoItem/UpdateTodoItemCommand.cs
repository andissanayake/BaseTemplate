using System.ComponentModel.DataAnnotations;
using BaseTemplate.Domain.Enums;

namespace BaseTemplate.Application.TodoItems.Commands.UpdateTodoItem;

[Authorize]
public record UpdateTodoItemCommand : IRequest<bool>
{
    public int Id { get; init; }

    [Required(ErrorMessage = "Title is required.")]
    [MaxLength(200, ErrorMessage = "The title cannot exceed 200 characters.")]
    public string? Title { get; init; }
    public string? Note { get; init; }
    public DateTimeOffset? Reminder { get; set; }
    public PriorityLevel? Priority { get; set; }
} 