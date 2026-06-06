    namespace HolidayPlanner.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Username { get; set; }
        // no PasswordHash
    }
}