﻿namespace Domain.Entities;

public class TodoItem
{
    public Guid Id { get; set; }
    public string Note { get; set; } = null!;
    public bool Done { get; set; }
    public Priority Priority { get; set; }
    public Guid TodoListId { get; set; }
    public TodoList TodoList { get; set; } = null!;
}