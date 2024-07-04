using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoList.Api.Controllers;
using TodoList.Api.Data;
using TodoList.Api.Models;

namespace TodoList.Api.Repository
{
    public class TodoListRepository : ITodoListRepository
    {
        private readonly TodoContext _context;
        private ILogger<TodoItemsController> _logger;
        public TodoListRepository(TodoContext context,ILogger<TodoItemsController> logger)
        {
            this._context = context;
            this._logger = logger;
        }

          public async Task<TodoItem> GetTodoItemByIdAsync(Guid Id)
        {
            return await _context.TodoItems.FindAsync(Id);
        }

        public async Task<List<TodoItem>> GetTodoItemsAsync()
        {
            return await _context.TodoItems.Where(x => !x.IsCompleted).ToListAsync();
        }

        public async Task<TodoItem> CreateToDoListItemAsync(TodoItem todoItem)
        {
            _context.TodoItems.Add(todoItem);
            await _context.SaveChangesAsync();
            return todoItem;
        }


        public async void PutToDoListItemAsync(Guid id, TodoItem todoItem)
        {
            var dbToDoItem = await _context.TodoItems.FindAsync(id);
            dbToDoItem.IsCompleted = todoItem.IsCompleted;

             await _context.SaveChangesAsync();

        }

        public async Task<bool> TodoItemDescriptionExistsAsync(string description)
        {
            return await _context.TodoItems
                   .AnyAsync(x => x.Description.ToLowerInvariant() == description.ToLowerInvariant() && !x.IsCompleted);
        }

        public async Task<bool> TodoItemIdExistsAsync(Guid id)
        {
            return await _context.TodoItems.AnyAsync(x => x.Id == id);
        }

    }
}
