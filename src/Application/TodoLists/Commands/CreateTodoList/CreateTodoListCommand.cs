using System.ComponentModel.DataAnnotations;
using BaseTemplate.Domain.ValueObjects;

namespace BaseTemplate.Application.TodoLists.Commands.CreateTodoList;

[Authorize]
public record CreateTodoListCommand : IRequest<int>
{
    [Required(ErrorMessage = "Title is required.")]
    [MaxLength(200, ErrorMessage = "The title cannot exceed 200 characters.")]
    public required string Title { get; init; }
    public string? Colour { get; init; }
} 