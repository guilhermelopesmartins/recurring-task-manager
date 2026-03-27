using System;
using RecurringTaskManager.Domain;

namespace RecurringTaskManager.API.DTO;

public class CreateTaskDto
{
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public PeriodType PeriodType { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }
}