﻿using Application.Common.Interfaces.Persistence;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;

namespace Infrastructure.Persistence.Repositories;

internal sealed class TodoListRepository :
    Repository<TodoList>, ITodoListRepository
{
    public TodoListRepository(TodoDbContext context) : base(context)
    {
    }

    public async Task<bool> IsTitleExists(string title)
    {
        return await DbContext.TodoLists
            .AnyAsync(tl => tl.Title == title);
    }

    public async Task<bool> TodoListExists(Guid todoListId)
    {
        return await DbContext.TodoLists
            .AnyAsync(tl => tl.Id == todoListId);
    }

    public async Task<List<TodoList>> GetUserTodoLists(Guid userId)
    {
        return await DbContext.TodoLists
            .Include(tl =>
                tl.TodoItems)
            .Where(tl => tl.UserId == userId && !tl.IsTodayTodoList)
            .ToListAsync();
    }

    public async Task<TodoList?> GetUserTodayTodolist(Guid userId)
    {
        return await DbContext.TodoLists
            .Include(tl => tl.TodoItems)
            .FirstOrDefaultAsync(tl => tl.UserId == userId
                                       && tl.IsTodayTodoList == true
                                       && tl.CreatedAt.Date == DateTime.UtcNow.Date);
    }

    public async Task<TodoList?> GetTodoListByIdWithItems(Guid todoListId)
    {
        return await DbContext.TodoLists
            .Include(tl => tl.TodoItems)
            .FirstOrDefaultAsync(tl => tl.Id == todoListId);
    }
    
    public async Task RemovePrevTodayTodoLists(Guid userId)
    {
        var prevTodayTodoLists = await DbContext.TodoLists
            .Where(tl => tl.UserId == userId && tl.IsTodayTodoList)
            .ToListAsync();
        
        DbContext.RemoveRange(prevTodayTodoLists);
    }

    public async Task<TodoList> CreateNewTodayTodoList(Guid userId)
    {
        var todayTodolist = new TodoList
        {
            Id = Guid.NewGuid(),
            Title = "Today",
            TodoItems = new List<TodoItem>(),
            UserId = userId,
            CreatedAt = DateTime.UtcNow,
            IsTodayTodoList = true
        };

        await AddAsync(todayTodolist);

        return todayTodolist;
    }
}