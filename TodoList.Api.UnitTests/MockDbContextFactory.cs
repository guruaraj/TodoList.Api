using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoList.Api.Data;
using TodoList.Api.Models;

namespace TodoList.Api.UnitTests
{
    public class MockDbContextFactory
    {
        public TodoContext DbContextFactory()
        {
            var options = new DbContextOptionsBuilder<TodoContext>().UseInMemoryDatabase("InMem").Options;

            var context = new TodoContext(options);
            context.Database.EnsureCreated();
            context.AddRange(
                new TodoItem() { Id = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa2"), Description = "test1", IsCompleted=false },
                new TodoItem() { Id = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa3"), Description = "test2", IsCompleted = true },
                new TodoItem() { Id = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa4"), Description = "test3", IsCompleted = false },
                new TodoItem() { Id = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa5"), Description = "test4", IsCompleted = false },
                new TodoItem() { Id = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6"), Description = "test6", IsCompleted = true }
                );
            context.SaveChanges();
            return context;
        }
    }
}
