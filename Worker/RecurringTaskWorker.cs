using Hangfire;
using RecurringTaskManager.Application.Service;
using RecurringTaskManager.Domain;
using RecurringTaskManager.Infrastructure.Persistence;

namespace RecurringTaskManager.Worker;

public class RecurringTaskWorker
{
    private readonly IRecurringTaskRepository _repository;
    private readonly IBackgroundJobClient _backgroundJobClient;
    private readonly IRecurringJobManager _recurringJobManager;

    public RecurringTaskWorker(IRecurringTaskRepository repository, IBackgroundJobClient backgroundJobClient, IRecurringJobManager recurringJobManager)
    {
        _repository = repository;
        _backgroundJobClient = backgroundJobClient;
        _recurringJobManager = recurringJobManager;
    }

    public async Task CreateJobAsync(Guid taskId)
    {
        Console.WriteLine($"Creating job for taskId: {taskId.ToString()}");
        var task = await _repository.GetTaskWithCyclesAsync(taskId);
        if (task == null) {
            RecurringJob.RemoveIfExists(taskId.ToString());
            return;
        }

        var hour = 0;
        var minute = 0;
        string cronExpression;

        switch (task.PeriodType)
        {
            case PeriodType.Daily:
                cronExpression = task.StartDate is null ? Cron.Daily() : $"{minute} {hour} * * *";
                break;
            case PeriodType.Weekly:
                var dow = (int?)task.StartDate?.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc).DayOfWeek;
                cronExpression = task.StartDate is null ? Cron.Weekly() : $"{minute} {hour} * * {dow}";
                break;
            case PeriodType.Monthly:
                var day = task.StartDate?.Day;
                cronExpression = task.StartDate is null ? Cron.Monthly() : $"{minute} {hour} {day} * *";
                break;
            default:
                throw new InvalidOperationException("Invalid PeriodType");
        }

        _recurringJobManager.AddOrUpdate<IRecurringTaskService>(
            taskId.ToString(),
            s => s.ExecuteCycleAsync(taskId),
            cronExpression
        );

        if (task.EndDate.HasValue)
        {
            var endDateTimeUtc = task.EndDate.Value.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
            _backgroundJobClient.Schedule(
                () => RecurringJob.RemoveIfExists(taskId.ToString()),
                endDateTimeUtc
            );
        }
    }

    public async Task RemoveJobAsync(Guid taskId)
    {
        var task = await _repository.GetTaskWithCyclesAsync(taskId);

        if (task is null)
            return;

        _recurringJobManager.RemoveIfExists(taskId.ToString());
    }
}