using Microsoft.EntityFrameworkCore;
using RecurringTaskManager.Domain;

namespace RecurringTaskManager.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public DbSet<RecurringTaskManager.Domain.RecurringTask> RecurringTasks => Set<RecurringTaskManager.Domain.RecurringTask>();
    public DbSet<TaskCycle> TaskCycles => Set<TaskCycle>();

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<RecurringTask>(entity =>
        {
            entity.ToTable("recurring_tasks");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(x => x.Description);

            entity.Property(x => x.PeriodType)
                .HasConversion<int>();

            entity.Property(x => x.StartDate);

            entity.Property(x => x.EndDate);

            entity.Property(x => x.IsActive);

            entity.Property(x => x.CreatedAt);

            entity.HasMany(x => x.Cycles)
                .WithOne()
                .HasForeignKey(x => x.TaskId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<TaskCycle>(entity =>
        {
            entity.ToTable("task_cycles");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.TaskId);

            entity.Property(x => x.StartDate);

            entity.Property(x => x.EndDate);

            entity.Property(x => x.Status)
                .HasConversion<int>();

            entity.Property(x => x.CreatedAt);
        });
    }
}