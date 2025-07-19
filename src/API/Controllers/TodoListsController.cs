using BaseTemplate.Application.Common.Models;
using BaseTemplate.Application.TodoLists.Commands.CreateTodoList;
using BaseTemplate.Application.TodoLists.Commands.DeleteTodoList;
using BaseTemplate.Application.TodoLists.Commands.GetTodoListById;
using BaseTemplate.Application.TodoLists.Commands.PurgeTodoLists;
using BaseTemplate.Application.TodoLists.Commands.UpdateTodoList;
using BaseTemplate.Application.TodoLists.Queries;
using BaseTemplate.Application.TodoLists.Queries.GetTodos;
using BaseTemplate.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BaseTemplate.API.Controllers;

[Authorize]
public class TodoListsController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<Result<TodosVm>>> Get()
    {
        return await SendAsync(new GetTodosQuery());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Result<TodoListDto>>> GetById(int id)
    {
        return await SendAsync(new GetTodoListByIdQuery(id));
    }

    [HttpPost]
    public async Task<ActionResult<Result<int>>> Create(CreateTodoListCommand command)
    {
        return await SendAsync(command);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Result<bool>>> Update(int id, UpdateTodoListCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest(Result<bool>.Validation("ID mismatch"));
        }

        return await SendAsync(command);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<Result<bool>>> Delete(int id)
    {
        return await SendAsync(new DeleteTodoListCommand(id));
    }

    [HttpDelete("purge")]
    [Authorize(policy: Policies.CanPurge)]
    public async Task<ActionResult<Result<bool>>> Purge()
    {
        return await SendAsync(new PurgeTodoListsCommand());
    }
}
