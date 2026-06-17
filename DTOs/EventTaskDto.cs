namespace CSE325_Team_2.DTOs
{
    public class EventTaskDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public bool IsCompleted { get; set; }
        public string? Assignee { get; set; }
        public int PlanId { get; set; }
    }
}