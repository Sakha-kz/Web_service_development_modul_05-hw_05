using Microsoft.AspNetCore.Mvc;
using TaskManagerClient.Models;
using TaskManagerClient.Services;

namespace TaskManagerClient.Controllers;

public class TasksController : Controller
{
    private readonly ITaskApiService _apiService;
    private readonly ILogger<TasksController> _logger;

    public TasksController(ITaskApiService apiService, ILogger<TasksController> logger)
    {
        _apiService = apiService;
        _logger = logger;
    }

    // GET: /Tasks or /
    public async Task<IActionResult> Index()
    {
        var tasks = await _apiService.GetAllAsync();
        return View(tasks);
    }

    // GET: /Tasks/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var task = await _apiService.GetByIdAsync(id);
        if (task == null)
        {
            TempData["ErrorMessage"] = $"Задача с указанным ID ({id}) не найдена.";
            return RedirectToAction(nameof(Index));
        }

        return View(task);
    }

    // GET: /Tasks/Create
    public IActionResult Create()
    {
        return View(new TaskItem());
    }

    // POST: /Tasks/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TaskItem task)
    {
        if (!ModelState.IsValid)
        {
            return View(task);
        }

        var (success, errorMessage) = await _apiService.CreateAsync(task);
        if (success)
        {
            TempData["SuccessMessage"] = $"Задача '{task.Title}' успешно создана!";
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError(string.Empty, errorMessage ?? "Не удалось создать задачу.");
        return View(task);
    }

    // GET: /Tasks/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var task = await _apiService.GetByIdAsync(id);
        if (task == null)
        {
            TempData["ErrorMessage"] = $"Задача с указанным ID ({id}) не найдена.";
            return RedirectToAction(nameof(Index));
        }

        return View(task);
    }

    // POST: /Tasks/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, TaskItem task)
    {
        if (id != task.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(task);
        }

        var (success, errorMessage) = await _apiService.UpdateAsync(task);
        if (success)
        {
            TempData["SuccessMessage"] = $"Задача '{task.Title}' успешно обновлена!";
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError(string.Empty, errorMessage ?? "Не удалось обновить задачу.");
        return View(task);
    }

    // GET: /Tasks/Delete/5 (Страница подтверждения удаления)
    public async Task<IActionResult> Delete(int id)
    {
        var task = await _apiService.GetByIdAsync(id);
        if (task == null)
        {
            TempData["ErrorMessage"] = $"Задача с указанным ID ({id}) не найдена.";
            return RedirectToAction(nameof(Index));
        }

        return View(task);
    }

    // POST: /Tasks/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var (success, errorMessage) = await _apiService.DeleteAsync(id);
        if (success)
        {
            TempData["SuccessMessage"] = $"Задача с ID {id} успешно удалена!";
        }
        else
        {
            TempData["ErrorMessage"] = errorMessage ?? $"Не удалось удалить задачу с ID {id}.";
        }

        return RedirectToAction(nameof(Index));
    }
}
