using RecurringTaskManager.Domain;

namespace RecurringTaskManager.API.DTO
{
    public class TaskCycleDto
    {
        public Guid Id { get; set; }
        public Guid TaskId { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public TaskCycleStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
