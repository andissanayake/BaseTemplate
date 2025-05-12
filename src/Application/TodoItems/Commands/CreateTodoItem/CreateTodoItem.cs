using System.ComponentModel.DataAnnotations;
using BaseTemplate.Application.Common.Interfaces;
using BaseTemplate.Application.Common.Models;
using BaseTemplate.Application.Common.RequestHandler;
using BaseTemplate.Domain.Entities;
using BaseTemplate.Domain.Enums;

namespace BaseTemplate.Application.TodoItems.Commands.CreateTodoItem;

//[Authorize]
public record CreateTodoItemCommand : IRequest<int>
{
    public int ListId { get; init; }

    [MaxLength(200, ErrorMessage = "The title cannot exceed 200 characters.")]
    public string? Title { get; init; }
    public string? Note { get; init; }
    public DateTimeOffset? Reminder { get; set; }
    public PriorityLevel? Priority { get; set; }
}

public class CreateTodoItemCommandHandler : IRequestHandler<CreateTodoItemCommand, int>
{
    private readonly IUnitOfWorkFactory _factory;
    public CreateTodoItemCommandHandler(IUnitOfWorkFactory factory)
    {
        _factory = factory;
    }

    public async Task<Result<int>> HandleAsync(CreateTodoItemCommand request, CancellationToken cancellationToken)
    {
        using var uow = _factory.Create();
        var entity = new TodoItem
        {
            ListId = request.ListId,
            Title = request.Title,
            Note = request.Note,
            Reminder = request.Reminder?.UtcDateTime,
            Priority = request.Priority ?? PriorityLevel.None,
            Done = false
        };
        await uow.InsertAsync(entity);
        return Result<int>.Success(entity.Id);
    }
}
