namespace HolidayPlanner.Model
{
    public class EventTask
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public bool IsCompleted { get; set; }
        public required Plan Plan { get; set; }
        public string? Assignee { get; set; }
    
    }
}