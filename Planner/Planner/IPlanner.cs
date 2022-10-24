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

public class TodoListCreate
{
    [Required]
    [StringLength(255, MinimumLength = 1)]
    public string Name { get; set; }
    public Todo[] Tasks { get; set; }
}

public class TodoListUpdate
{
    [Required]
    public string Id { get; set; }
    [Required]
    [StringLength(255, MinimumLength = 1)]
    public string Name { get; set; }
    public Todo[] Tasks { get; set; }
}

public class TodoList
{
    public string Id { get; set; }
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; }
    public Todo[] Tasks { get; set; }
    public User Owner { get; set; }
    public User[] SharedTo { get; set; }
}

public class Todo
{
    public string Title { get; set; }
}

public class User
{
    public string Id { get; set; }
}