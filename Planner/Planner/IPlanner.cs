using System;
using System.ComponentModel.DataAnnotations;

namespace Planner;

public interface IPlanner
{
    Task<TodoList[]> GetListsByUserIdAsync(string userId, int limit, int offset);
    Task<TodoList?> FindListAsync(string listId);
    Task<TodoList> AddListAsync(string userId, TodoListCreate list);
    Task UpdateListAsync(string userId, TodoListUpdate list);
    Task DeleteListAsync(string userId, string listId);
    Task ShareAsync(string userId, string listId, string targetUserId);
    Task RevokeAsync(string userId, string listId, string targetUserId);
}