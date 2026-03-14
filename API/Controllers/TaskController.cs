using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RecurringTaskManager.API.DTO;
using RecurringTaskManager.Application.Service;

namespace RecurringTaskManagerManager.API.Controller;

[ApiController]
[Route("api/tasks")]
public class TaskController : ControllerBase
{
    private readonly IRecurringTaskService _recurringTaskService;

    public TaskController(IRecurringTaskService recurringTaskService)
    {
        _recurringTaskService = recurringTaskService;
    }
    
    [HttpPost]
    public async Task<ActionResult<TaskResponseDto>> CreateTask(CreateTaskDto dto)
    {
        var task = await _recurringTaskService.CreateTaskAsync(
            dto.Title,
            dto.Description,
            dto.PeriodType,
            dto.StartDate,
            dto.EndDate
        );

        var response = new TaskResponseDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            PeriodType = task.PeriodType,
            IsActive = task.IsActive,
            CreatedAt = task.CreatedAt
        };

        return CreatedAtAction(nameof(GetTask), new { id = task.Id }, response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TaskResponseDto>> GetTask(Guid id)
    {
        var task = await _recurringTaskService.GetTaskAsync(id);

        if (task == null)
            return NotFound();

        var response = new TaskResponseDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            PeriodType = task.PeriodType,
            IsActive = task.IsActive,
            CreatedAt = task.CreatedAt
        };

        return Ok(response);
    }
    
    [HttpPost("cycles/complete")]
    public async Task<IActionResult> CompleteCycle(CompleteCycleDto dto)
    {
        await _recurringTaskService.CompleteCycleAsync(dto.CycleId);

        return NoContent();
    }

    [HttpPatch("{id}/deactivate")]
    public async Task<IActionResult> DeactivateTask(Guid id)
    {
        await _recurringTaskService.DeactivateTaskAsync(id);

        return NoContent();
    }
}