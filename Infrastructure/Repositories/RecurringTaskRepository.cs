using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

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

    public async Task<RecurringTaskManager.Domain.RecurringTask?> GetByIdAsync(Guid id)
    {
        return await _context.RecurringTasks
            .Include("_cycles")
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<RecurringTaskManager.Domain.RecurringTask?> GetByCycleIdAsync(Guid cycleId)
    {
        return await _context.RecurringTasks
            .Include("_cycles")
            .FirstOrDefaultAsync(t =>
                t.Cycles.Any(c => c.Id == cycleId));
    }
    public async Task UpdateAsync(RecurringTaskManager.Domain.RecurringTask task)
    {
        _context.RecurringTasks.Update(task);
        await _context.SaveChangesAsync();
    }
}