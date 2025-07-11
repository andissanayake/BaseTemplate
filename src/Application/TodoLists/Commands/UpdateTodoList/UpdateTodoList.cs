using System.ComponentModel.DataAnnotations;
using BaseTemplate.Domain.ValueObjects;

namespace BaseTemplate.Application.TodoLists.Commands.UpdateTodoList;

[Authorize]
public record UpdateTodoListCommand : IRequest<bool>
{
    public int Id { get; init; }

    [Required(ErrorMessage = "Title is required.")]
    [MaxLength(200, ErrorMessage = "The title cannot exceed 200 characters.")]
    public required string Title { get; init; }
    public string? Colour { get; init; }
}

public class UpdateTodoListCommandHandler : IRequestHandler<UpdateTodoListCommand, bool>
{
    private readonly IUnitOfWorkFactory _factory;

    public UpdateTodoListCommandHandler(IUnitOfWorkFactory factory)
    {
        _factory = factory;
    }

    //public override async Task<Result<bool>> ValidateAsync(UpdateTodoListCommand request, CancellationToken cancellationToken)
    //{
    //    var val = ModelValidator.Validate(request);
    //    if (!val.IsValid)
    //        return Result<bool>.Validation("validation", val.Errors);

    //    var sql = "SELECT COUNT(1) FROM TodoList WHERE Title = @Title and Id != @Id";
    //    using var uow = _factory.CreateUOW();
    //    var count = await uow.QueryFirstOrDefaultAsync<int>(sql, new { request.Title, request.Id });

    //    if (count > 0)
    //        return Result<bool>.Validation("validation", new Dictionary<string, string[]>
    //        {
    //            ["Title"] = ["The title already exists."]
    //        });
    //    return Result<bool>.Success(true);
    //}
    public async Task<Result<bool>> HandleAsync(UpdateTodoListCommand request, CancellationToken cancellationToken)
    {
        using var uow = _factory.Create();
        var entity = await uow.GetAsync<TodoList>(request.Id);

        if (entity is null)
        {
            return Result<bool>.NotFound($"TodoList with id {request.Id} not found.");
        }

        entity.Title = request.Title;
        entity.Colour = request.Colour ?? Colour.White.Code;
        await uow.UpdateAsync(entity);
        return Result<bool>.Success(true);
    }
}
