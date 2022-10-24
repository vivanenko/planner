using System;
using System.ComponentModel.DataAnnotations;

namespace Planner;

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