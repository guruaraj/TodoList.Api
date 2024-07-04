using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading;
using TodoList.Api.Controllers;
using TodoList.Api.Data;
using TodoList.Api.Models;
using TodoList.Api.Repository;
using Xunit;


namespace TodoList.Api.UnitTests
{
    public class DummyTestShould
    {
        ITodoListRepository service = new Mock<ITodoListRepository>().Object;
        ILogger<TodoItemsController> logger = new Mock<ILogger<TodoItemsController>>().Object;
        public TodoContext context;

        TodoItemsController controller;
        public DummyTestShould()
        {
            var ContextFactory = new MockDbContextFactory();
            context = ContextFactory.DbContextFactory();
            service = new TodoListRepository(context, logger);

            this.controller = new TodoItemsController(service, logger);
        }
        [Fact]
        public void GetallToDoItems_Empty()
        {
            var result = controller.GetTodoItemsAsync();

            Assert.NotNull(result);
            context.Database.EnsureDeleted();
        }

        [Fact]
        public async void Get_OneItem_ById()
        {
            string id = "3fa85f64-5717-4562-b3fc-2c963f66afa2";
            var res = await controller.GetTodoItemAsync(Guid.Parse(id));
            var statusCode = ((Microsoft.AspNetCore.Mvc.OkObjectResult)res).StatusCode;
            var responseItem = (TodoItem)((Microsoft.AspNetCore.Mvc.ObjectResult)res).Value;

            // Assert
            Assert.Equal(Guid.Parse(id), responseItem.Id);
            Assert.Equal(StatusCodes.Status200OK, statusCode);
            context.Database.EnsureDeleted();
        }

        [Fact]
        public async void Get_OneItem_ById_NotFound()
        {
            string id = "3fa85f64-5717-4562-b3fc-2c963f66afa8";
            var res = await controller.GetTodoItemAsync(Guid.Parse(id));
            var statusCode = ((Microsoft.AspNetCore.Mvc.NotFoundResult)res).StatusCode;

            Assert.Equal(StatusCodes.Status404NotFound, statusCode);
            context.Database.EnsureDeleted();
        }

        [Fact]
        public async void Get_OneItem_ById_EmptyGuid()
        {
            var res = await controller.GetTodoItemAsync(Guid.Empty);
            var statusCode = ((Microsoft.AspNetCore.Mvc.NotFoundResult)res).StatusCode;

            Assert.Equal(StatusCodes.Status404NotFound, statusCode);
            context.Database.EnsureDeleted();
        }


        [Fact]
        public async void Insert_OneItem()
        {
            string id = "3fa85f64-5717-4562-b3fc-2c963f66afa9";
            var todoItem = new TodoItem() { Id = Guid.Parse(id), Description = "FirstTodo", IsCompleted = false };
            var res = await controller.PostTodoItemAsync(todoItem);
            var responseItem = (TodoItem)((ObjectResult)res).Value;
            var statusCode = ((ObjectResult)res).StatusCode;

            Assert.Equal(Guid.Parse(id), responseItem.Id);
            Assert.Equal(StatusCodes.Status201Created, statusCode);
            context.Database.EnsureDeleted();
        }

        [Fact]
        public async void Insert_OneItem_EmptyGuid()
        {
            var eguid = Guid.Empty;
            var todoItem = new TodoItem() { Id = eguid, Description = "FirstTodo", IsCompleted = false };
            var res = await controller.PostTodoItemAsync(todoItem);
            var errorMessage = ((ObjectResult)res).Value;

            var statusCode = ((ObjectResult)res).StatusCode;

            Assert.Equal(StatusCodes.Status400BadRequest, statusCode);
            Assert.Equal("Guid is Empty", errorMessage);
            context.Database.EnsureDeleted();
        }


        [Fact]
        public async void Insert_OneItem_ExistingGuid()
        {
            string id = "3fa85f64-5717-4562-b3fc-2c963f66afa2";
            var todoItem = new TodoItem() { Id = Guid.Parse(id), Description = "FirstTodo", IsCompleted = false };
            var res = await controller.PostTodoItemAsync(todoItem);

            var statusCode = ((ObjectResult)res).StatusCode;

            var errorMessage = ((ObjectResult)res).Value;

            Assert.Equal(StatusCodes.Status400BadRequest, statusCode);
            Assert.Equal("Guid already exists", errorMessage);
            context.Database.EnsureDeleted();
        }

        [Fact]
        public async void Insert_OneItem_NoDescription()
        {
            string id = "3fa85f64-5717-4562-b3fc-2c963f66afb8";
            var todoItem = new TodoItem() { Id = Guid.Parse(id), Description = "", IsCompleted = false };
            var res = await controller.PostTodoItemAsync(todoItem);

            var statusCode = ((ObjectResult)res).StatusCode;

            var errorMessage = ((ObjectResult)res).Value;

            Assert.Equal(StatusCodes.Status400BadRequest, statusCode);
            Assert.Equal("Description is required", errorMessage);
            context.Database.EnsureDeleted();
        }

        [Fact]
        public async void Insert_OneItem_ExistingDescription()
        {
            string id = "3fa85f64-5717-4562-b3fc-2c963f66afc8";
            var todoItem = new TodoItem() { Id = Guid.Parse(id), Description = "test1", IsCompleted = false };
            var res = await controller.PostTodoItemAsync(todoItem);

            var statusCode = ((ObjectResult)res).StatusCode;

            var errorMessage = ((ObjectResult)res).Value;

            Assert.Equal(StatusCodes.Status400BadRequest, statusCode);
            Assert.Equal("Description already exists", errorMessage);
            context.Database.EnsureDeleted();
        }

        [Fact]
        public async void Put_OneItem()
        {
            string id = "3fa85f64-5717-4562-b3fc-2c963f66afa2";

            var todoItem = new TodoItem() { Id = Guid.Parse(id), Description = "FirstTodo", IsCompleted = true };

            var res = await controller.PutTodoItemAsync(Guid.Parse(id), todoItem);

            Assert.Equal(StatusCodes.Status204NoContent, ((StatusCodeResult)res).StatusCode);
            context.Database.EnsureDeleted();
        }

        [Fact]
        public async void Put_OneItem_IdDonotMatch()
        {
            string id = "3fa85f64-5717-4562-b3fc-2c963f66afa2";
            string idSecond = "3fa85f64-5717-4562-b3fc-2c963f66afa3";

            var todoItem = new TodoItem() { Id = Guid.Parse(id), Description = "FirstTodo", IsCompleted = true };

            var res = await controller.PutTodoItemAsync(Guid.Parse(idSecond), todoItem);

            var statusCode = ((ObjectResult)res).StatusCode;

            var errorMessage = ((ObjectResult)res).Value;

            Assert.Equal(StatusCodes.Status400BadRequest, statusCode);
            Assert.Equal("Id do not match", errorMessage);
            context.Database.EnsureDeleted();
        }

        [Fact]
        public async void Put_OneItem_IdDoesNotExist()
        {
            string id = "3fa85f64-5717-4562-b3fc-2c963f66afa9";

            var todoItem = new TodoItem() { Id = Guid.Parse(id), Description = "FirstTodo", IsCompleted = true };

            var res = await controller.PutTodoItemAsync(Guid.Parse(id), todoItem);

            var statusCode = ((ObjectResult)res).StatusCode;

            var errorMessage = ((ObjectResult)res).Value;

            Assert.Equal(StatusCodes.Status400BadRequest, statusCode);
            Assert.Equal("Guid doesnot exists", errorMessage);
            context.Database.EnsureDeleted();

        }

    }
}
