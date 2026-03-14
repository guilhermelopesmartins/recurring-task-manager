using System;
using RecurringTaskManager.Domain;

namespace RecurringTaskManager.API.DTO;

public class TaskResponseDto
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public PeriodType PeriodType { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }
}