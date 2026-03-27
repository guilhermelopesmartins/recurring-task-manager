using System;

namespace RecurringTaskManager.Domain;

public class TaskCycle
{
    public Guid Id { get; private set; }
    public Guid TaskId { get; private set; }
    public DateOnly StartDate { get; private set; }
    public DateOnly EndDate { get; private set; }
    public TaskCycleStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    
    private  TaskCycle() {}

    public TaskCycle(Guid taskId, DateOnly startDate, DateOnly endDate)
    {
        Id = Guid.NewGuid();
        TaskId = taskId;
        StartDate = startDate;
        EndDate = endDate;
        Status = TaskCycleStatus.Active;
        CreatedAt = DateTime.UtcNow;
    }

    public void CompleteCycle()
    {
        if (Status != TaskCycleStatus.Active)
        {
            throw new Exception("Cannot complete cycle while task is not active");
        }
        Status = TaskCycleStatus.Completed;
    }
    
    public void FailCycle()
    {
        if (Status != TaskCycleStatus.Active)
        {
            throw new Exception("Cannot fail cycle unless it's active");
        }

        Status = TaskCycleStatus.Failed;
    }
}