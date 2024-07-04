using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NuGet.Protocol.Core.Types;
using System;
using System.Linq;
using System.Threading.Tasks;
//using System.Web.Http;
using TodoList.Api.Data;
using TodoList.Api.Models;
using TodoList.Api.Repository;

namespace TodoList.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoItemsController : ControllerBase
    {
        private readonly ITodoListRepository _repository;
        private readonly ILogger<TodoItemsController> _logger;

        public TodoItemsController(ITodoListRepository repository, ILogger<TodoItemsController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        // GET: api/TodoItems
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTodoItemsAsync()
        {
            var results = await _repository.GetTodoItemsAsync();
            return Ok(results);
        }

        // GET: api/TodoItems/...
        [HttpGet("{id:Guid}", Name = "GetTodoItemAsync")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTodoItemAsync([FromRoute] Guid id)
        {
            var result = await _repository.GetTodoItemByIdAsync(id);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        // POST: api/TodoItems 
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PostTodoItemAsync([FromBody] TodoItem todoItem)
        {
            if (todoItem.Id==Guid.Empty)
            {
                return BadRequest("Guid is Empty");
            }
            else if (await TodoItemIdExistsAsync(todoItem.Id))
            {
                return BadRequest("Guid already exists");
            }
            else if (string.IsNullOrEmpty(todoItem?.Description))
            {
                return BadRequest("Description is required");
            }
            else if (await TodoItemDescriptionExistsAsync(todoItem.Description))
            {
                return BadRequest("Description already exists");
            } 

            await _repository.CreateToDoListItemAsync(todoItem);
             
            return CreatedAtRoute(nameof(GetTodoItemAsync), new { id = todoItem.Id }, todoItem);
        }

        // PUT: api/TodoItems/... 
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PutTodoItemAsync(Guid id, TodoItem todoItem)
        {
            if (id != todoItem.Id)
            {
                return BadRequest("Id do not match");
            }
            if (await TodoItemIdExistsAsync(id) == false)
            {
                return BadRequest("Guid doesnot exists");
            }
            //else if (await TodoItemDescriptionExistsAsync(todoItem.Description))
            //{
            //    return BadRequest("Description already exists");
            //}

            try
            {
                _repository.PutToDoListItemAsync(id,todoItem);
            }
            catch (DbUpdateConcurrencyException)
            {
                if ( ! await TodoItemIdExistsAsync(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        private async Task<bool> TodoItemIdExistsAsync(Guid id)
        {
            return await _repository.TodoItemIdExistsAsync(id);
        }

        private async Task<bool> TodoItemDescriptionExistsAsync(string description)
        {
            return await _repository.TodoItemDescriptionExistsAsync(description);

        }
    }
}
