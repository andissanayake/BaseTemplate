using System.ComponentModel.DataAnnotations;
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

    //public async Task<Result<int>> HandleAsync(CreateTodoItemCommand request, CancellationToken cancellationToken)
    //{
    //    var _cs = "Server=postgres;Database=BaseTemplate;User Id=postgres;Password=JOker1988++++;Pooling=true;MinPoolSize=10;MaxPoolSize=100;";

    //    //using var uow = _factory.Create();
    //    using var uow = new NpgsqlConnection(_cs);
    //    var sql = @"
    //    INSERT INTO todo_item (list_id, title, note, reminder, priority, done)
    //    VALUES (@ListId, @Title, @Note, @Reminder, @Priority, @Done)
    //    RETURNING id;
    //    ";

    //    var entity = new
    //    {
    //        ListId = request.ListId,
    //        Title = request.Title,
    //        Note = request.Note,
    //        Reminder = request.Reminder?.UtcDateTime,
    //        Priority = request.Priority ?? PriorityLevel.None,
    //        Done = false
    //    };

    //    var newId = await uow.ExecuteScalarAsync<int>(sql, entity);
    //    return Result<int>.Success(newId);
    //}

}
