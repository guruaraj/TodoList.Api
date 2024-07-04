using System;
using System.ComponentModel.DataAnnotations;

namespace TodoList.Api.Models
{
    public class TodoItem
    {
        [Key]
        [RegularExpression("^((?!00000000-0000-0000-0000-000000000000).)*$", ErrorMessage = "Cannot use default Guid")]
        public Guid Id { get; set; }

        public string Description { get; set; }

        public bool IsCompleted { get; set; }
    }
}
