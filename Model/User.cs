namespace CSE325_Team_2.Model
{
    public class User
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Username { get; set; }
        public required string PasswordHash { get; set; }
    }
}