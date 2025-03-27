using BaseTemplate.Application.Common.Interfaces;

namespace BaseTemplate.Application.TodoLists.Commands.CreateTodoList;

public class CreateTodoListCommandValidator : AbstractValidator<CreateTodoListCommand>
{
    private readonly IUnitOfWorkFactory _factory;

    public CreateTodoListCommandValidator(IUnitOfWorkFactory factory)
    {
        _factory = factory;

        RuleFor(v => v.Title)
            .NotEmpty()
            .MaximumLength(200)
            .MustAsync(BeUniqueTitle)
                .WithMessage("'{PropertyName}' must be unique.")
                .WithErrorCode("Unique");
    }

    public async Task<bool> BeUniqueTitle(string title, CancellationToken cancellationToken)
    {
        //return await _context.TodoLists
        //    .AllAsync(l => l.Title != title, cancellationToken);
        throw new Exception("Not implemented");
    }
}
