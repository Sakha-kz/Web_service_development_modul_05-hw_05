using Microsoft.AspNetCore.Mvc;
using TaskManagerClient.Models;

namespace TaskManagerClient.Controllers.Api;

[ApiController]
[Route("api/tasks")]
public class TasksApiController : ControllerBase
{
    private static readonly List<TaskItem> _tasks = new()
    {
        new TaskItem { Id = 1, Title = "Изучить HttpClient", Description = "Изучить фабрику IHttpClientFactory и работу сокетов", IsCompleted = true },
        new TaskItem { Id = 2, Title = "Реализовать Web API", Description = "Разработать контроллеры и Swagger документацию", IsCompleted = false },
        new TaskItem { Id = 3, Title = "Изучить PATCH", Description = "Разобраться с частичным обновлением данных", IsCompleted = false }
    };
    private static readonly object _lock = new();

    [HttpGet]
    public IActionResult GetAll()
    {
        lock (_lock)
        {
            return Ok(_tasks.ToList());
        }
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        lock (_lock)
        {
            var task = _tasks.FirstOrDefault(t => t.Id == id);
            if (task == null)
            {
                return NotFound(new { message = $"Задача с ID {id} не найдена." });
            }
            return Ok(task);
        }
    }

    [HttpPost]
    public IActionResult Create([FromBody] TaskItem task)
    {
        if (task == null || string.IsNullOrWhiteSpace(task.Title))
        {
            return BadRequest(new { message = "Название задачи является обязательным." });
        }

        lock (_lock)
        {
            task.Id = _tasks.Count > 0 ? _tasks.Max(t => t.Id) + 1 : 1;
            _tasks.Add(task);
            return CreatedAtAction(nameof(GetById), new { id = task.Id }, task);
        }
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] TaskItem updated)
    {
        if (updated == null || string.IsNullOrWhiteSpace(updated.Title))
        {
            return BadRequest(new { message = "Некорректные данные для обновления задачи." });
        }

        lock (_lock)
        {
            var task = _tasks.FirstOrDefault(t => t.Id == id);
            if (task == null)
            {
                return NotFound(new { message = $"Задача с ID {id} не найдена." });
            }

            task.Title = updated.Title;
            task.Description = updated.Description;
            task.IsCompleted = updated.IsCompleted;

            return Ok(task);
        }
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        lock (_lock)
        {
            var task = _tasks.FirstOrDefault(t => t.Id == id);
            if (task == null)
            {
                return NotFound(new { message = $"Задача с ID {id} не найдена." });
            }

            _tasks.Remove(task);
            return Ok(new { message = $"Задача с ID {id} успешно удалена." });
        }
    }
}
