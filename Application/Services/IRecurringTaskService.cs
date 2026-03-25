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

    Task<RecurringTask?> GetTaskAsync(Guid id);
    Task ExecuteCycleAsync(Guid taskId, DateOnly cycleStartDate);
}