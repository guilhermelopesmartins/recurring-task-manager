using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using RecurringTaskManager.Domain;

namespace RecurringTaskManager.Infrastructure.Persistence;

public interface IRecurringTaskRepository
{
    Task AddAsync(Domain.RecurringTask task);

    Task<Domain.RecurringTask?> GetTaskWithCyclesAsync(Guid id);

    Task<Domain.RecurringTask?> GetByCycleIdAsync(Guid cycleId);

    Task UpdateAsync(Domain.RecurringTask task);

    Task<Domain.RecurringTask?> GetByIdAsync(Guid id);

    Task<List<Domain.RecurringTask>> GetAllAsync();

    Task<List<Domain.RecurringTask>> GetAllWithCyclesAsync();
    Task SaveChangesAsync();

    Task<bool> CycleExistsAsync(Guid taskId, DateOnly startDate);

    Task<Domain.TaskCycle?> GetActiveCycleAsync(Guid taskId);

    Task AddCycleAsync(Domain.TaskCycle cycle);

    Task UpdateCycleAsync(Domain.TaskCycle cycle);
}