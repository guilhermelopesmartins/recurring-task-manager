using System;
using System.Threading.Tasks;
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

    public async Task RunAsync(Guid taskId, DateOnly cycleStartDate)
    {
        var task = await _repository.GetByIdAsync(taskId);
        if (task == null) {
            RecurringJob.RemoveIfExists(taskId.ToString());
            return;
        }

        // In future, we can give users the ability to specify the time of day for task execution. For now, we will default to 00:00 (midnight).
        var hour = 0;
        var minute = 0;
        string cronExpression;

        switch (task.PeriodType)
        {
            case PeriodType.Daily:
                cronExpression = task.StartDate is not null ? $"{minute} {hour} * * *" : Cron.Daily();
                break;
            case PeriodType.Weekly:
                var dow = task.StartDate?.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc).DayOfWeek;
                cronExpression = task.StartDate is not null ? $"{minute} {hour} * * {(int)dow}" : Cron.Weekly();
                break;
            case PeriodType.Monthly:
                var day = task.StartDate?.Day;
                cronExpression = task.StartDate is not null ? $"{minute} {hour} {day} * *" : Cron.Monthly();
                break;
            default:
                throw new InvalidOperationException("Invalid PeriodType");
        }

        _recurringJobManager.AddOrUpdate<IRecurringTaskService>(
            taskId.ToString(),
            s => s.ExecuteCycleAsync(taskId, cycleStartDate),
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
}