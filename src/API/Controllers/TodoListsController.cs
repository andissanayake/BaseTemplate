using BaseTemplate.Application.TodoLists.Commands.CreateTodoList;
using BaseTemplate.Application.TodoLists.Commands.DeleteTodoList;
using BaseTemplate.Application.TodoLists.Commands.GetTodoListById;
using BaseTemplate.Application.TodoLists.Commands.PurgeTodoLists;
using BaseTemplate.Application.TodoLists.Commands.UpdateTodoList;
using BaseTemplate.Application.TodoLists.Queries;
using BaseTemplate.Application.TodoLists.Queries.GetTodos;
using MediatorS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BaseTemplate.API.Controllers;

[Authorize]
public class TodoListsController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<Result<TodosVm>>> Get()
    {
        return await Mediator.SendAsync(new GetTodosQuery());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Result<TodoListDto>>> GetById(int id)
    {
        return await Mediator.SendAsync(new GetTodoListByIdQuery(id));
    }

    [HttpPost]
    public async Task<ActionResult<Result<int>>> Create(CreateTodoListCommand command)
    {
        return await Mediator.SendAsync(command);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Result<bool>>> Update(int id, UpdateTodoListCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest();
        }

        await Mediator.SendAsync(command);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<Result<bool>>> Delete(int id)
    {
        await Mediator.SendAsync(new DeleteTodoListCommand(id));

        return NoContent();
    }

    [HttpDelete("purge")]
    public async Task<ActionResult<Result<bool>>> Purge()
    {
        await Mediator.SendAsync(new PurgeTodoListsCommand());
        return NoContent();
    }
}
