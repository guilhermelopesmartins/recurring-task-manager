using System;
using RecurringTaskManager.Domain;

namespace RecurringTaskManager.API.DTO;

public class TaskWithCyclesResponseDto
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public PeriodType PeriodType { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public List<TaskCycleDto> Cycles { get; set; } = new List<TaskCycleDto>();
}