﻿using System.ComponentModel.DataAnnotations;
using BaseTemplate.Application.Common.Interfaces;
using BaseTemplate.Domain.Entities;
using BaseTemplate.Domain.ValueObjects;

namespace BaseTemplate.Application.TodoLists.Commands.UpdateTodoList;

public record UpdateTodoListCommand : IRequest<bool>
{
    public int Id { get; init; }

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

    public async Task<Result<bool>> ValidateAsync(UpdateTodoListCommand request, CancellationToken cancellationToken)
    {
        var val = ModelValidator.Validate(request);
        if (!val.IsValied)
            return Result<bool>.Validation("validation", val.Errors);

        var sql = "SELECT COUNT(1) FROM TodoList WHERE Title = @Title";
        using var uow = _factory.CreateUOW();
        var count = await uow.QueryFirstOrDefaultAsync<int>(sql, new { request.Title });

        if (count > 0)
            return Result<bool>.Validation("validation", new Dictionary<string, string[]>
            {
                ["Title"] = ["The title already exists."]
            });
        return Result<bool>.Success(true);
    }
    public async Task<Result<bool>> HandleAsync(UpdateTodoListCommand request, CancellationToken cancellationToken)
    {
        using var uow = _factory.CreateUOW();
        var entity = await uow.GetAsync<TodoList>(request.Id);

        if (entity is null)
        {
            return Result<bool>.NotFound($"TodoList with id {request.Id} not found.");
        }

        entity.Title = request.Title;
        entity.Colour = request.Colour ?? Colour.White.Code;
        await uow.UpdateAsync(entity);
        uow.Commit();
        return Result<bool>.Success(true);
    }
}
