﻿using BaseTemplate.Application.TodoLists.Commands.CreateTodoList;
using BaseTemplate.Application.TodoLists.Commands.DeleteTodoList;
using BaseTemplate.Application.TodoLists.Commands.GetTodoListById;
using BaseTemplate.Application.TodoLists.Commands.PurgeTodoLists;
using BaseTemplate.Application.TodoLists.Commands.UpdateTodoList;
using BaseTemplate.Application.TodoLists.Queries;
using BaseTemplate.Application.TodoLists.Queries.GetTodos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BaseTemplate.API.Controllers;

[Authorize]
public class TodoListsController : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<TodosVm>> Get()
    {
        return await Mediator.Send(new GetTodosQuery());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TodoListDto>> GetById(int id)
    {
        return await Mediator.Send(new GetTodoListByIdQuery(id));
    }

    [HttpPost]
    public async Task<ActionResult<int>> Create(CreateTodoListCommand command)
    {
        return await Mediator.Send(command);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, UpdateTodoListCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest();
        }

        await Mediator.Send(command);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        await Mediator.Send(new DeleteTodoListCommand(id));

        return NoContent();
    }

    [HttpDelete("purge")]
    public async Task<ActionResult> Purge()
    {
        await Mediator.Send(new PurgeTodoListsCommand());
        return NoContent();
    }
}
