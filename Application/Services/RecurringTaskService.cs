using Hangfire;
using RecurringTaskManager.Domain;
using RecurringTaskManager.Infrastructure.Persistence;
using RecurringTaskManager.Worker;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RecurringTaskManager.Application.Service;

public class RecurringTaskService : IRecurringTaskService
{
    private readonly IRecurringTaskRepository _recurringTaskRepository;
    private readonly IBackgroundJobClient _backgroundJobClient;

    public RecurringTaskService(IRecurringTaskRepository recurringTaskRepository, IBackgroundJobClient backgroundJobClient)
    {
        _recurringTaskRepository = recurringTaskRepository;
        _backgroundJobClient = backgroundJobClient;
    }
    public async Task<RecurringTask> CreateTaskAsync(string title, string? description, PeriodType periodType, DateOnly? startDate, DateOnly? endDate)
    {
        var task = new RecurringTask(
            title,
            description,
            periodType,
            startDate,
            endDate
        );
        await _recurringTaskRepository.AddAsync(task);

        var delay = TimeSpan.Zero;
        if (task.StartDate.HasValue && task.StartDate is not null)
        {
            var startDateTimeUtc = task.StartDate?.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
            var computedDelay = startDateTimeUtc - DateTime.UtcNow;
            delay = (TimeSpan)(computedDelay > TimeSpan.Zero ? computedDelay : TimeSpan.Zero);
        }
        _backgroundJobClient.Schedule<RecurringTaskWorker>(
                worker => worker.CreateJobAsync(task.Id),
                delay
            );

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

        _backgroundJobClient.Enqueue<RecurringTaskWorker>(
            worker => worker.RemoveJobAsync(task.Id)
        );
    }

    public async Task DeactivateTaskAsync(Guid taskId)
    {
        var task = await _recurringTaskRepository.GetTaskWithCyclesAsync(taskId);

        if (task == null)
            throw new Exception("Task not found");

        task.Deactivate();

        await _recurringTaskRepository.UpdateAsync(task);

        _backgroundJobClient.Enqueue<RecurringTaskWorker>(
            worker => worker.RemoveJobAsync(task.Id)
        );
    }

    public async Task ActivateTaskAsync(Guid taskId)
    {
        var task = await _recurringTaskRepository.GetTaskWithCyclesAsync(taskId);

        if (task == null)
            throw new Exception("Task not found");

        task.Activate();

        await _recurringTaskRepository.UpdateAsync(task);

        _backgroundJobClient.Enqueue<RecurringTaskWorker>(
            worker => worker.CreateJobAsync(task.Id)
        );
    }

    public async Task<RecurringTask?> GetTaskWithCyclesAsync(Guid id)
    {
        return await _recurringTaskRepository.GetTaskWithCyclesAsync(id);
    }

    public async Task<RecurringTask?> GetTaskAsync(Guid id)
    {
        return await _recurringTaskRepository.GetByIdAsync(id);
    }

    public async Task<List<RecurringTask>> GetAllAsync()
    {
        return await _recurringTaskRepository.GetAllAsync();
    }

    public async Task<List<RecurringTask>> GetAllWithCyclesAsync(TaskCycleStatus? filterStatus = null)
    {
        var tasks = await _recurringTaskRepository.GetAllWithCyclesAsync();

        if (!filterStatus.HasValue)
            return tasks;

        foreach (var t in tasks)
        {
            var filtered = t.Cycles.Where(c => c.Status == filterStatus.Value).ToList();
        }

        return tasks;
    }

    public async Task ExecuteCycleAsync(Guid taskId)
    {
        Console.WriteLine($"Executing cycle for task {taskId} at {DateTime.UtcNow}");

        var task = await _recurringTaskRepository.GetByIdAsync(taskId);

        if (task == null)
            throw new Exception("Task not found");

        if (!task.IsActive)
            return;

        var cycleStartDate = DateOnly.FromDateTime(DateTime.UtcNow);

        if (await _recurringTaskRepository.CycleExistsAsync(taskId, cycleStartDate))
            return;

        DateOnly endCycle;
        switch (task.PeriodType)
        {
            case PeriodType.Daily:
                endCycle = cycleStartDate;
                break;
            case PeriodType.Weekly:
                endCycle = cycleStartDate.AddDays(6);
                break;
            case PeriodType.Monthly:
                endCycle = cycleStartDate.AddMonths(1).AddDays(-1);
                break;
            default:
                throw new InvalidOperationException("Invalid PeriodType");
        }

        var active = await _recurringTaskRepository.GetActiveCycleAsync(taskId);
        if (active is not null)
        {
            active.FailCycle();
            await _recurringTaskRepository.UpdateCycleAsync(active);
        }

        var newCycle = task.AddCycle(cycleStartDate, endCycle);
        await _recurringTaskRepository.AddCycleAsync(newCycle);
    }
}