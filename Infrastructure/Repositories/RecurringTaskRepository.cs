using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using RecurringTaskManager.Domain;

namespace RecurringTaskManager.Infrastructure.Persistence;

public class RecurringTaskRepository : IRecurringTaskRepository
{
    private readonly AppDbContext _context;

    public RecurringTaskRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(RecurringTaskManager.Domain.RecurringTask task)
    {
        await _context.RecurringTasks.AddAsync(task);
        await _context.SaveChangesAsync();
    }

    public async Task<RecurringTaskManager.Domain.RecurringTask?> GetTaskWithCyclesAsync(Guid id)
    {
        return await _context.RecurringTasks
            .Include(x => x.Cycles)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<RecurringTaskManager.Domain.RecurringTask?> GetByCycleIdAsync(Guid cycleId)
    {
        return await _context.RecurringTasks
            .Include(x => x.Cycles)
            .FirstOrDefaultAsync(t =>
                t.Cycles.Any(c => c.Id == cycleId));
    }
    public async Task UpdateAsync(RecurringTaskManager.Domain.RecurringTask task)
    {
        _context.RecurringTasks.Update(task);
        await _context.SaveChangesAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<RecurringTaskManager.Domain.RecurringTask?> GetByIdAsync(Guid id)
    {
        return await _context.RecurringTasks
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<RecurringTaskManager.Domain.RecurringTask>> GetAllAsync()
    {
        return await _context.RecurringTasks
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<RecurringTaskManager.Domain.RecurringTask>> GetAllWithCyclesAsync()
    {
        return await _context.RecurringTasks
            .Include(x => x.Cycles)
            .AsNoTracking()
            .ToListAsync();
    }
    public async Task<bool> CycleExistsAsync(Guid taskId, DateOnly startDate)
    {
        return await _context.TaskCycles
            .AnyAsync(c => c.TaskId == taskId && c.StartDate == startDate);
    }

    public async Task<RecurringTaskManager.Domain.TaskCycle?> GetActiveCycleAsync(Guid taskId)
    {
        return await _context.TaskCycles
            .FirstOrDefaultAsync(c => c.TaskId == taskId && c.Status == TaskCycleStatus.Active);
    }

    public async Task AddCycleAsync(RecurringTaskManager.Domain.TaskCycle cycle)
    {
        await _context.TaskCycles.AddAsync(cycle);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateCycleAsync(RecurringTaskManager.Domain.TaskCycle cycle)
    {
        _context.TaskCycles.Update(cycle);
        await _context.SaveChangesAsync();
    }
    
}