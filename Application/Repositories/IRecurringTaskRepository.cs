using System;
using System.Threading.Tasks;
using RecurringTaskManager.Domain;

namespace RecurringTaskManager.Infrastructure.Persistence;

public interface IRecurringTaskRepository
{
    Task AddAsync(Domain.RecurringTask task);

    Task<Domain.RecurringTask?> GetByIdAsync(Guid id);

    Task<Domain.RecurringTask?> GetByCycleIdAsync(Guid cycleId);

    Task UpdateAsync(Domain.RecurringTask task);
}