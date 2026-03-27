using System;
using System.Threading.Tasks;
using RecurringTaskManager.Domain;

namespace RecurringTaskManager.Application.Service;

public interface IRecurringTaskService
{
    Task<RecurringTask> CreateTaskAsync(
        string title,
        string? description,
        PeriodType periodType,
        DateOnly? startDate,
        DateOnly? endDate
    );

    Task CompleteCycleAsync(Guid cycleId);

    Task DeactivateTaskAsync(Guid taskId);
    Task ActivateTaskAsync(Guid taskId);

    Task<RecurringTask?> GetTaskWithCyclesAsync(Guid id);
    Task<RecurringTask?> GetTaskAsync(Guid id);

    Task<List<RecurringTask>> GetAllAsync();

    Task<List<RecurringTask>> GetAllWithCyclesAsync(TaskCycleStatus? filterStatus = null);

    Task ExecuteCycleAsync(Guid taskId);
}