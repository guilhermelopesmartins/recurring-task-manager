using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RecurringTaskManager.API.DTO;
using RecurringTaskManager.Application.Service;
using RecurringTaskManager.Domain;

namespace RecurringTaskManagerManager.API.Controller;

[ApiController]
[Tags("Tasks")]
[Route("api/tasks")]
public class TaskController : ControllerBase
{
    private readonly IRecurringTaskService _recurringTaskService;

    public TaskController(IRecurringTaskService recurringTaskService)
    {
        _recurringTaskService = recurringTaskService;
    }
    
    [HttpPost]
    public async Task<ActionResult<TaskWithCyclesResponseDto>> CreateTask(CreateTaskDto dto)
    {
        var task = await _recurringTaskService.CreateTaskAsync(
            dto.Title,
            dto.Description,
            dto.PeriodType,
            dto.StartDate,
            dto.EndDate
        );

        var response = new TaskWithCyclesResponseDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            PeriodType = task.PeriodType,
            IsActive = task.IsActive,
            CreatedAt = task.CreatedAt
        };

        return CreatedAtAction(nameof(GetTaskWithCycles), new { id = task.Id }, response);
    }

    [HttpGet("{id}/cycles")]
    public async Task<ActionResult<TaskWithCyclesResponseDto>> GetTaskWithCycles(Guid id)
    {
        var task = await _recurringTaskService.GetTaskWithCyclesAsync(id);

        if (task == null)
            return NotFound();

        var response = new TaskWithCyclesResponseDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            PeriodType = task.PeriodType,
            IsActive = task.IsActive,
            CreatedAt = task.CreatedAt,
            StartDate = task.StartDate,
            EndDate = task.EndDate,
            Cycles = task.Cycles.Select(c => new TaskCycleDto
            {
                Id = c.Id,
                TaskId = c.TaskId,
                StartDate = c.StartDate,
                EndDate = c.EndDate,
                Status = c.Status,
                CreatedAt = c.CreatedAt
            }).ToList()
        };

        return Ok(response);
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

    [HttpGet]
    public async Task<ActionResult<List<TaskResponseDto>>> GetAll()
    {
        var tasks = await _recurringTaskService.GetAllAsync();
        var response = tasks.Select(t => new TaskResponseDto
        {
            Id = t.Id,
            Title = t.Title,
            Description = t.Description,
            PeriodType = t.PeriodType,
            IsActive = t.IsActive,
            CreatedAt = t.CreatedAt
        }).ToList();

        return Ok(response);
    }

    [HttpGet("with-cycles")]
    public async Task<ActionResult<List<TaskWithCyclesResponseDto>>> GetAllWithCycles([FromQuery] TaskCycleStatus? status)
    {
        var tasks = await _recurringTaskService.GetAllWithCyclesAsync(status);

        var response = tasks.Select(task => new TaskWithCyclesResponseDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            PeriodType = task.PeriodType,
            IsActive = task.IsActive,
            CreatedAt = task.CreatedAt,
            StartDate = task.StartDate,
            EndDate = task.EndDate,
            Cycles = task.Cycles
                .Where(c => !status.HasValue || c.Status == status.Value)
                .Select(c => new TaskCycleDto
                {
                    Id = c.Id,
                    TaskId = c.TaskId,
                    StartDate = c.StartDate,
                    EndDate = c.EndDate,
                    Status = c.Status,
                    CreatedAt = c.CreatedAt
                }).ToList()
        }).ToList();

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

    [HttpPatch("{id}/activate")]
    public async Task<IActionResult> ActivateTask(Guid id)
    {
        await _recurringTaskService.ActivateTaskAsync(id);

        return NoContent();
    }
}