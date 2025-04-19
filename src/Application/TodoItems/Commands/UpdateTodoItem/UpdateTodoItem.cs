using System.ComponentModel.DataAnnotations;
using BaseTemplate.Application.Common.Interfaces;
using BaseTemplate.Domain.Entities;
using BaseTemplate.Domain.Enums;

namespace BaseTemplate.Application.TodoItems.Commands.UpdateTodoItem;

public record UpdateTodoItemCommand : IRequest<bool>
{
    public int Id { get; init; }

    [MaxLength(200, ErrorMessage = "The title cannot exceed 200 characters.")]
    public string? Title { get; init; }
    public string? Note { get; init; }
    public DateTimeOffset? Reminder { get; set; }
    public PriorityLevel? Priority { get; set; }
}

public class UpdateTodoItemCommandHandler : IRequestHandler<UpdateTodoItemCommand, bool>
{
    private readonly IUnitOfWorkFactory _factory;
    public UpdateTodoItemCommandHandler(IUnitOfWorkFactory factory)
    {
        _factory = factory;
    }
    public async Task<Result<bool>> HandleAsync(UpdateTodoItemCommand request, CancellationToken cancellationToken)
    {
        using var uow = _factory.CreateUOW();
        var entity = await uow.GetAsync<TodoItem>(request.Id);

        if (entity is null)
        {
            return Result<bool>.NotFound($"TodoItem with id {request.Id} not found.");
        }

        entity.Title = request.Title;
        entity.Title = request.Title;
        entity.Note = request.Note;
        entity.Reminder = request.Reminder;
        entity.Done = false;
        entity.Priority = request.Priority ?? PriorityLevel.None;

        await uow.UpdateAsync(entity);
        uow.Commit();
        return Result<bool>.Success(true);
    }
}
