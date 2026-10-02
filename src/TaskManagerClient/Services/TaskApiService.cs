using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using TaskManagerClient.Models;

namespace TaskManagerClient.Services;

public class TaskApiService : ITaskApiService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<TaskApiService> _logger;
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    public TaskApiService(IHttpClientFactory httpClientFactory, ILogger<TaskApiService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    private HttpClient CreateClient() => _httpClientFactory.CreateClient("TaskApi");

    public async Task<List<TaskItem>> GetAllAsync()
    {
        try
        {
            var client = CreateClient();
            var response = await client.GetAsync("api/tasks");

            if (response.IsSuccessStatusCode)
            {
                var tasks = await response.Content.ReadFromJsonAsync<List<TaskItem>>(_jsonOptions);
                return tasks ?? new List<TaskItem>();
            }

            _logger.LogWarning("Failed to retrieve tasks. StatusCode: {StatusCode}", response.StatusCode);
            return new List<TaskItem>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while fetching all tasks from Web API");
            return new List<TaskItem>();
        }
    }

    public async Task<TaskItem?> GetByIdAsync(int id)
    {
        try
        {
            var client = CreateClient();
            var response = await client.GetAsync($"api/tasks/{id}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<TaskItem>(_jsonOptions);
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogWarning("Task with ID {TaskId} was not found (404)", id);
                return null;
            }

            _logger.LogWarning("Error fetching task ID {TaskId}. StatusCode: {StatusCode}", id, response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception while fetching task with ID {TaskId}", id);
            return null;
        }
    }

    public async Task<(bool Success, string? ErrorMessage)> CreateAsync(TaskItem task)
    {
        try
        {
            var client = CreateClient();
            var response = await client.PostAsJsonAsync("api/tasks", task);

            if (response.IsSuccessStatusCode)
            {
                return (true, null);
            }

            var errorBody = await response.Content.ReadAsStringAsync();
            var errorMsg = response.StatusCode switch
            {
                HttpStatusCode.BadRequest => $"Некорректные данные задачи (400): {errorBody}",
                HttpStatusCode.InternalServerError => "Внутренняя ошибка сервера API (500)",
                _ => $"Ошибка API ({response.StatusCode}): {errorBody}"
            };

            _logger.LogWarning("Failed to create task. StatusCode: {StatusCode}, Message: {Message}", response.StatusCode, errorMsg);
            return (false, errorMsg);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception while creating task");
            return (false, $"Сетевая ошибка при обращении к Web API: {ex.Message}");
        }
    }

    public async Task<(bool Success, string? ErrorMessage)> UpdateAsync(TaskItem task)
    {
        try
        {
            var client = CreateClient();
            var response = await client.PutAsJsonAsync($"api/tasks/{task.Id}", task);

            if (response.IsSuccessStatusCode)
            {
                return (true, null);
            }

            var errorBody = await response.Content.ReadAsStringAsync();
            var errorMsg = response.StatusCode switch
            {
                HttpStatusCode.NotFound => "Задача с указанным ID не найдена (404)",
                HttpStatusCode.BadRequest => $"Некорректные данные задачи (400): {errorBody}",
                HttpStatusCode.InternalServerError => "Внутренняя ошибка сервера API (500)",
                _ => $"Ошибка API ({response.StatusCode}): {errorBody}"
            };

            _logger.LogWarning("Failed to update task ID {TaskId}. StatusCode: {StatusCode}", task.Id, response.StatusCode);
            return (false, errorMsg);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception while updating task ID {TaskId}", task.Id);
            return (false, $"Сетевая ошибка при обращении к Web API: {ex.Message}");
        }
    }

    public async Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id)
    {
        try
        {
            var client = CreateClient();
            var response = await client.DeleteAsync($"api/tasks/{id}");

            if (response.IsSuccessStatusCode)
            {
                return (true, null);
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return (false, "Задача с указанным ID не найдена (404)");
            }

            var errorBody = await response.Content.ReadAsStringAsync();
            return (false, $"Ошибка при удалении задачи ({response.StatusCode}): {errorBody}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception while deleting task with ID {TaskId}", id);
            return (false, $"Сетевая ошибка при обращении к Web API: {ex.Message}");
        }
    }
}
