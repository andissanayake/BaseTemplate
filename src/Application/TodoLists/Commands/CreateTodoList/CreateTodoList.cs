using System.ComponentModel.DataAnnotations;
using BaseTemplate.Application.Common.Interfaces;
using BaseTemplate.Domain.Entities;
using BaseTemplate.Domain.ValueObjects;

namespace BaseTemplate.Application.TodoLists.Commands.CreateTodoList;

public record CreateTodoListCommand : IRequest<int>
{
    [MaxLength(200, ErrorMessage = "The title cannot exceed 200 characters.")]
    public required string Title { get; init; }
    public string? Colour { get; init; }
}

public class CreateTodoListCommandHandler : IRequestHandler<CreateTodoListCommand, int>
{
    private readonly IUnitOfWorkFactory _factory;

    public CreateTodoListCommandHandler(IUnitOfWorkFactory factory)
    {
        _factory = factory;
    }
    public async Task<Result<int>> ValidateAsync(CreateTodoListCommand request, CancellationToken cancellationToken)
    {
        var val = ModelValidator.Validate(request);
        if (!val.IsValied)
            return Result<int>.Validation("validation", val.Errors);

        var sql = "SELECT COUNT(1) FROM TodoList WHERE Title = @Title";
        using var uow = _factory.CreateUOW();
        var count = await uow.QueryFirstOrDefaultAsync<int>(sql, new { request.Title });

        if (count > 0)
            return Result<int>.Validation("validation", new Dictionary<string, string[]>
            {
                ["Title"] = ["The title already exists."]
            });
        return Result<int>.Success(0);
    }
    public async Task<Result<int>> HandleAsync(CreateTodoListCommand request, CancellationToken cancellationToken)
    {
        var entity = new TodoList();
        entity.Title = request.Title;

        if (request.Colour is not null)
            entity.Colour = Colour.From(request.Colour).Code;

        using var uow = _factory.CreateUOW();
        await uow.InsertAsync(entity);
        uow.Commit();

        return Result<int>.Success(entity.Id);
    }
}
