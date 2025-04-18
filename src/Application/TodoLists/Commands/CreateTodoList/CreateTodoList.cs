using BaseTemplate.Application.Common.Interfaces;
using BaseTemplate.Domain.Entities;
using BaseTemplate.Domain.ValueObjects;

namespace BaseTemplate.Application.TodoLists.Commands.CreateTodoList;

public record CreateTodoListCommand : IRequest<int>
{
    public string Title { get; init; }
    public string Colour { get; init; }
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
        /*
         
        RuleFor(v => v.Title)
            .NotEmpty()
            .MaximumLength(200)
            .MustAsync(BeUniqueTitle)
                .WithMessage("'{PropertyName}' must be unique.")
                .WithErrorCode("Unique");
    }

    public async Task<bool> BeUniqueTitle(string title, CancellationToken cancellationToken)
    {
        var sql = "SELECT COUNT(1) FROM TodoList WHERE Title = @Title";
        using var uow = _factory.CreateUOW();
        var count = await uow.QueryFirstOrDefaultAsync<int>(sql, new { Title = title });
        return count == 0;
    }
         */
        return Result<int>.Success(0);
    }
    public async Task<Result<int>> HandleAsync(CreateTodoListCommand request, CancellationToken cancellationToken)
    {
        var entity = new TodoList();
        entity.Title = request.Title;
        entity.Colour = Colour.From(request.Colour).Code;

        using var uow = _factory.CreateUOW();
        await uow.InsertAsync(entity);
        uow.Commit();

        return Result<int>.Success(entity.Id);
    }
}
