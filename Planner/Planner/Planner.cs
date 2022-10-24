using System;
using System.Collections.Generic;
using MongoDB.Driver;

namespace Planner;

public class Planner : IPlanner
{
    private class TodoListDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public Todo[] Tasks { get; set; }
        public string OwnerId { get; set; }
        public string[] SharedTo { get; set; }
    }

    private readonly IMongoClient _mongoClient;
    private readonly IMongoDatabase _mongoDatabase;
    private readonly IMongoCollection<TodoListDto> _todoLists;

    public Planner(IMongoClient mongoClient)
    {
        _mongoClient = mongoClient;
        _mongoDatabase = _mongoClient.GetDatabase("planner");
        _todoLists = _mongoDatabase.GetCollection<TodoListDto>("todoLists");
    }

    public async Task<TodoList[]> GetListsByUserIdAsync(string userId, int limit, int offset)
    {
        var docs = await _todoLists
            .Find(e => e.OwnerId == userId || e.SharedTo.Contains(userId))
            .SortBy(e => e.CreatedAt)
            .Limit(limit)
            .Skip(offset)
            .ToListAsync();
        return docs.Select(MapDocToList).ToArray();
    }

    public async Task<TodoList?> FindListAsync(string listId)
    {
        var doc = await _todoLists.Find(e => e.Id == listId).SingleOrDefaultAsync();
        return doc == null ? null : MapDocToList(doc);
    }

    public async Task<TodoList> AddListAsync(string userId, TodoListCreate list)
    {
        var now = DateTime.UtcNow;
        var doc = new TodoListDto
        {
            Id = Guid.NewGuid().ToString(),
            Name = list.Name,
            CreatedAt = now,
            Tasks = list.Tasks,
            OwnerId = userId,
            SharedTo = Array.Empty<string>()
        };
        await _todoLists.InsertOneAsync(doc);

        return MapDocToList(doc);
    }

    public async Task UpdateListAsync(string userId, TodoListUpdate list)
    {
        var update = Builders<TodoListDto>.Update
            .Set(e => e.Name, list.Name)
            .Set(e => e.Tasks, list.Tasks);
        await _todoLists.UpdateOneAsync(e => e.Id == list.Id, update);
    }

    public async Task DeleteListAsync(string userId, string listId)
    {
        var doc = await _todoLists.Find(e => e.Id == listId).SingleOrDefaultAsync();
        if (doc.OwnerId != userId)
            throw new Exception("Not allowed. Only an owner can delete a list.");
        await _todoLists.DeleteOneAsync(e => e.Id == listId);
    }

    public async Task ShareAsync(string userId, string listId, string targetUserId)
    {
        var update = Builders<TodoListDto>.Update.AddToSet(e => e.SharedTo, targetUserId);
        await _todoLists.UpdateOneAsync(e => e.Id == listId, update);
    }

    public async Task RevokeAsync(string userId, string listId, string targetUserId)
    {
        var update = Builders<TodoListDto>.Update.Pull(e => e.SharedTo, targetUserId);
        await _todoLists.UpdateOneAsync(e => e.Id == listId, update);
    }

    private static TodoList MapDocToList(TodoListDto dto)
    {
        return new TodoList
        {
            Id = dto.Id,
            Name = dto.Name,
            CreatedAt = dto.CreatedAt,
            Tasks = dto.Tasks,
            Owner = new User { Id = dto.OwnerId },
            SharedTo = dto.SharedTo.Select(id => new User { Id = id }).ToArray()
        };
    }
}