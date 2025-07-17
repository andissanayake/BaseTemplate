using BaseTemplate.Application.Common.Models;
using BaseTemplate.Application.TodoItems.Commands.CreateTodoItem;
using BaseTemplate.Application.TodoItems.Commands.DeleteTodoItem;
using BaseTemplate.Application.TodoItems.Commands.UpdateTodoItem;
using BaseTemplate.Application.TodoItems.Commands.UpdateTodoItemStatus;
using BaseTemplate.Application.TodoItems.Queries.GetTodoItemsWithPagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BaseTemplate.API.Controllers;

[Authorize]
public class TodoItemsController : ApiControllerBase
{
    [HttpGet]

    public async Task<ActionResult<Result<PaginatedList<TodoItemBriefDto>>>> GetTodoItemsWithPagination([FromQuery] GetTodoItemsWithPaginationQuery query)
    {
        return await SendAsync(query);
    }

    [HttpPost]
    public async Task<ActionResult<Result<int>>> Create(CreateTodoItemCommand command)
    {
        return await SendAsync(command);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Result<bool>>> Update(int id, UpdateTodoItemCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest(Result<bool>.Validation("ID mismatch"));
        }

        return await SendAsync(command);
    }

    [HttpPut("[action]")]
    public async Task<ActionResult<Result<bool>>> UpdateItemStatus(int id, UpdateTodoItemStatusCommand command)
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
        return await SendAsync(new DeleteTodoItemCommand(id));
    }
}
