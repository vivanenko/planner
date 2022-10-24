using System;

namespace Planner;

public class TodoList
{
    public string Id { get; set; }
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; }
    public Todo[] Tasks { get; set; }
    public User Owner { get; set; }
    public List<User> SharedTo { get; set; }
}

public class Todo
{
    public string Title { get; set; }
}

public class User
{
    public string Id { get; set; }
}