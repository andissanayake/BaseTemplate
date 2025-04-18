using BaseTemplate.Application.Common.Interfaces;
using BaseTemplate.Domain.Entities;
using BaseTemplate.Domain.ValueObjects;

namespace BaseTemplate.Application.TodoLists.Commands.UpdateTodoList;

public record UpdateTodoListCommand : IRequest<bool>
{
    public int Id { get; init; }

    public string? Title { get; init; }
    public string? Colour { get; init; }
}

public class UpdateTodoListCommandHandler : IRequestHandler<UpdateTodoListCommand, bool>
{
    private readonly IUnitOfWorkFactory _factory;

    public UpdateTodoListCommandHandler(IUnitOfWorkFactory factory)
    {
        _factory = factory;
    }

    public async Task<Result<bool>> ValidateAsync(UpdateTodoListCommand request, CancellationToken cancellationToken)
    {
        /*
        RuleFor(v => v.Title)
            .NotEmpty()
            .MaximumLength(200)
            .MustAsync(BeUniqueTitle)
                .WithMessage("'{PropertyName}' must be unique.")
                .WithErrorCode("Unique");
    }

    public async Task<bool> BeUniqueTitle(UpdateTodoListCommand model, string title, CancellationToken cancellationToken)
    {
        var sql = "SELECT COUNT(1) FROM TodoList WHERE Title = @Title and Id != @Id";
        using var uow = _factory.CreateUOW();
        var count = await uow.QueryFirstOrDefaultAsync<int>(sql, new { Title = title, model.Id });
        return count == 0;
    }
         */
        return Result<bool>.Success(true);
    }
    public async Task<Result<bool>> HandleAsync(UpdateTodoListCommand request, CancellationToken cancellationToken)
    {
        using var uow = _factory.CreateUOW();
        var entity = await uow.GetAsync<TodoList>(request.Id);

        Guard.Against.NotFound(request.Id, entity);

        entity.Title = request.Title;
        entity.Colour = request.Colour ?? Colour.White.Code;
        await uow.UpdateAsync(entity);
        uow.Commit();
        return Result<bool>.Success(true);
    }
}
