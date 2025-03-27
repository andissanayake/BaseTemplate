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
        //return await _context.TodoLists
        //    .Where(l => l.Id != model.Id)
        //    .AllAsync(l => l.Title != title, cancellationToken);
        throw new Exception("Not implemented");
    }
}
