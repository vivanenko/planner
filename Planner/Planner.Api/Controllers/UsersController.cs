using Microsoft.AspNetCore.Mvc;

namespace Planner.Api.Controllers;

[ApiController]
[Route("users")]
public class UsersController : ControllerBase
{
    private readonly IPlanner _planner;

    public UsersController(IPlanner planner)
    {
        _planner = planner;
    }

    [HttpGet("{userId}/todolists")]
    public async Task<TodoList[]> GetTodoLists([FromRoute] string userId,
        [FromQuery] int limit = 20, [FromQuery] int offset = 0)
    {
        return await _planner.GetListsByUserIdAsync(userId, limit, offset);
    }

    [HttpGet("{userId}/todolists/{listId}")]
    public async Task<IActionResult> GetTodoList([FromRoute] string userId, [FromRoute] string listId)
    {
        var list = await _planner.FindListAsync(listId);
        if (list is null) return NotFound();
        return Ok(list);
    }

    [HttpPost("{userId}/todolists")]
    public async Task<TodoList> PostTodoList([FromRoute] string userId,
        [FromBody] TodoListCreate todoList)
    {
        return await _planner.AddListAsync(userId, todoList);
    }

    [HttpPut("{userId}/todolists/{listId}")]
    public async Task<IActionResult> PutTodoList([FromRoute] string userId,
        [FromRoute] string listId, [FromBody] TodoListUpdate todoList)
    {
        await _planner.UpdateListAsync(userId, todoList);
        return Ok();
    }

    [HttpDelete("{userId}/todolists/{listId}")]
    public async Task<IActionResult> DeleteTodoList([FromRoute] string userId,
        [FromRoute] string listId)
    {
        await _planner.DeleteListAsync(userId, listId);
        return Ok();
    }

    [HttpPut("{userId}/todolists/{listId}/shareTo/{targetUserId}")]
    public async Task<IActionResult> ShareTodoList([FromRoute] string userId,
        [FromRoute] string listId, [FromRoute] string targetUserId)
    {
        await _planner.ShareAsync(userId, listId, targetUserId);
        return Ok();
    }

    [HttpPut("{userId}/todolists/{listId}/revokeFrom/{targetUserId}")]
    public async Task<IActionResult> RevokeTodoList([FromRoute] string userId,
        [FromRoute] string listId, [FromRoute] string targetUserId)
    {
        await _planner.RevokeAsync(userId, listId, targetUserId);
        return Ok();
    }
}