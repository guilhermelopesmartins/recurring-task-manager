using System;
using System.Collections.Generic;

namespace RecurringTaskManager.Domain;

public class RecurringTask
{
    public Guid Id { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public PeriodType PeriodType { get; private set; }
    public DateOnly? StartDate { get; private set; }
    public DateOnly? EndDate { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    private readonly List<TaskCycle> _cycles = new();
    public IReadOnlyCollection<TaskCycle> Cycles => _cycles;
    private  RecurringTask() {}

    public RecurringTask(string title, string? description, PeriodType periodType, DateOnly? startDate, DateOnly? endDate)
    {
        Id = Guid.NewGuid();
        Title = title;
        Description = description;
        PeriodType = periodType;
        StartDate = startDate;
        EndDate = endDate;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    public TaskCycle AddCycle(DateOnly startDate, DateOnly endDate)
    {
        var cycle = new TaskCycle(Id, startDate, endDate);
        _cycles.Add(cycle);
        
        return cycle;
    }

    public void Deactivate()
    {
        IsActive = false;
    }
    public void Activate()
    {
        IsActive = true;
    }
}