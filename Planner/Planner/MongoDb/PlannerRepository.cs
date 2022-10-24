using System;
using System.Linq.Expressions;
using MongoDB.Driver;

namespace Planner.MongoDb;

public class PlannerRepository : IPlannerRepository
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

    public PlannerRepository(IMongoClient mongoClient)
    {
        _mongoClient = mongoClient;
        _mongoDatabase = _mongoClient.GetDatabase("planner");
        _todoLists = _mongoDatabase.GetCollection<TodoListDto>("todoLists");
    }

    public async Task<TodoList?> FindAsync(string listId)
    {
        var doc = await _todoLists.Find(e => e.Id == listId).SingleOrDefaultAsync();
        return doc == null ? null : MapDocToList(doc);
    }

    public async Task<TodoList> SaveAsync(TodoList list)
    {
        var doc = MapListToDoc(list);
        await _todoLists.ReplaceOneAsync(e => e.Id == list.Id, doc, new ReplaceOptions { IsUpsert = true });
        return list;
    }

    public async Task DeleteAsync(string listId)
    {
        await _todoLists.DeleteOneAsync(e => e.Id == listId);
    }

    public async Task<TodoList[]> GetUsersListsAsync(string userId, int limit, int offset)
    {
        var docs = await _todoLists
            .Find(e => e.OwnerId == userId || e.SharedTo.Contains(userId))
            .SortByDescending(e => e.CreatedAt)
            .Limit(limit)
            .Skip(offset)
            .ToListAsync();
        return docs.Select(MapDocToList).ToArray();
    }

    private static TodoListDto MapListToDoc(TodoList list)
    {
        return new TodoListDto
        {
            Id = list.Id,
            Name = list.Name,
            CreatedAt = list.CreatedAt,
            Tasks = list.Tasks,
            OwnerId = list.Owner.Id,
            SharedTo = list.SharedTo.Select(u => u.Id).ToArray()
        };
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
            SharedTo = dto.SharedTo.Select(id => new User { Id = id }).ToList()
        };
    }
}