using System;

namespace Planner;

public interface IPlannerRepository
{
    Task<TodoList?> FindAsync(string listId);
    Task<TodoList> SaveAsync(TodoList list);
    Task DeleteAsync(string listId);

    Task<TodoList[]> GetUsersListsAsync(string userId, int limit, int offset);
}