namespace HolidayPlanner.Model
{
    public class EventTask
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsCompleted { get; set; }
        public Plan Plan { get; set; }
        public User Assignee { get; set; }
    
    }
}