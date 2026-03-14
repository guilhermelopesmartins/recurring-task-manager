using System;
using System.Linq;
using System.Threading.Tasks;
using RecurringTaskManager.Domain;
using RecurringTaskManager.Infrastructure.Persistence;

namespace RecurringTaskManager.Application.Service;

public class RecurringTaskService : IRecurringTaskService
{
    private readonly IRecurringTaskRepository _recurringTaskRepository;

    public RecurringTaskService(IRecurringTaskRepository recurringTaskRepository)
    {
        _recurringTaskRepository = recurringTaskRepository;
    }
    public async Task<RecurringTask> CreateTaskAsync(string title, string? description, PeriodType periodType, DateOnly startDate, DateOnly? endDate)
    {
        var task = new RecurringTask(
            title,
            description,
            periodType,
            startDate,
            endDate
        );
        await _recurringTaskRepository.AddAsync(task);
        return task;
    }

    public async Task CompleteCycleAsync(Guid cycleId)
    {
        var task = await _recurringTaskRepository.GetByCycleIdAsync(cycleId);

        if (task == null)
            throw new Exception("Task not found for this cycle");

        var cycle = task.Cycles.FirstOrDefault(c => c.Id == cycleId);

        if (cycle == null)
            throw new Exception("Cycle not found");

        cycle.CompleteCycle();

        await _recurringTaskRepository.UpdateAsync(task);
    }

    public async Task DeactivateTaskAsync(Guid taskId)
    {
        var task = await _recurringTaskRepository.GetByIdAsync(taskId);

        if (task == null)
            throw new Exception("Task not found");

        task.Deactivate();

        await _recurringTaskRepository.UpdateAsync(task);
    }

    public async Task<RecurringTask?> GetTaskAsync(Guid id)
    {
        return await _recurringTaskRepository.GetByIdAsync(id);
    }
}