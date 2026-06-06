namespace HolidayPlanner.DTOs
{
    public class PlanDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public decimal Price { get; set; }
        public DateTime Date { get; set; }
        public required UserDto Creator { get; set; }
        public List<string> Participants { get; set; } = new();
        public List<UserDto> Collaborators { get; set; } = new();
    }
}