using System;
using System.Collections;
using System.Collections.Generic;
using MongoDB.Driver;

namespace Planner.Services;

public class Planner : IPlanner
{
    private readonly IPlannerRepository _plannerRepository;

    public Planner(IPlannerRepository plannerRepository)
    {
        _plannerRepository = plannerRepository;
    }

    public async Task<TodoList[]> GetListsByUserIdAsync(string userId, int limit, int offset)
    {
        return await _plannerRepository.GetUsersListsAsync(userId, limit, offset);
    }

    public async Task<TodoList?> FindListAsync(string listId)
    {
        return await _plannerRepository.FindAsync(listId);
    }

    public async Task<TodoList> AddListAsync(string userId, TodoListCreate list)
    {
        var now = DateTime.UtcNow;
        var newList = new TodoList
        {
            Id = Guid.NewGuid().ToString(),
            Name = list.Name,
            CreatedAt = now,
            Tasks = list.Tasks,
            Owner = new User { Id = userId },
            SharedTo = new List<User>()
        };
        await _plannerRepository.SaveAsync(newList);
        return newList;
    }

    public async Task UpdateListAsync(string userId, TodoListUpdate list)
    {
        var existingList = await _plannerRepository.FindAsync(list.Id);
        if (existingList == null)
            throw new Exception($"TodoList with id {list.Id} not found.");
        existingList.Name = list.Name;
        existingList.Tasks = list.Tasks;
        await _plannerRepository.SaveAsync(existingList);
    }

    public async Task DeleteListAsync(string userId, string listId)
    {
        var list = await _plannerRepository.FindAsync(listId);
        if (list?.Owner.Id != userId)
            throw new Exception("Not allowed. Only an owner can delete a list.");
        await _plannerRepository.DeleteAsync(listId);
    }

    public async Task ShareAsync(string userId, string listId, string targetUserId)
    {
        var existingList = await _plannerRepository.FindAsync(listId);
        if (existingList == null)
            throw new Exception($"TodoList with id {listId} not found.");
        existingList.SharedTo.Add(new User { Id = targetUserId });
        await _plannerRepository.SaveAsync(existingList);
    }

    public async Task RevokeAsync(string userId, string listId, string targetUserId)
    {
        var existingList = await _plannerRepository.FindAsync(listId);
        if (existingList == null)
            throw new Exception($"TodoList with id {listId} not found.");
        existingList.SharedTo.RemoveAll(u => u.Id == targetUserId);
        await _plannerRepository.SaveAsync(existingList);
    }
}