using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TodoList.Api.Models;

namespace TodoList.Api.Repository
{
    public interface ITodoListRepository
    {
        Task<List<TodoItem>> GetTodoItemsAsync();
        Task<TodoItem> GetTodoItemByIdAsync(Guid Id);
        void PutToDoListItemAsync(Guid id, TodoItem todoItem);
        Task<TodoItem> CreateToDoListItemAsync(TodoItem todoItem);
        Task<bool> TodoItemIdExistsAsync(Guid id);
        Task<bool> TodoItemDescriptionExistsAsync(string description);


    }
}
