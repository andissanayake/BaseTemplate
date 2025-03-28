using BaseTemplate.Application.Common.Interfaces;

namespace BaseTemplate.Application.TodoLists.Commands.UpdateTodoList;

public class UpdateTodoListCommandValidator : AbstractValidator<UpdateTodoListCommand>
{
    private readonly IUnitOfWorkFactory _factory;

    public UpdateTodoListCommandValidator(IUnitOfWorkFactory factory)
    {
        _factory = factory;

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
}
