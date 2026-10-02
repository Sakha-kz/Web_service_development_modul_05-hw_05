using TaskManagerClient.Models;

namespace TaskManagerClient.Services;

public interface ITaskApiService
{
    Task<List<TaskItem>> GetAllAsync();
    Task<TaskItem?> GetByIdAsync(int id);
    Task<(bool Success, string? ErrorMessage)> CreateAsync(TaskItem task);
    Task<(bool Success, string? ErrorMessage)> UpdateAsync(TaskItem task);
    Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id);
}
