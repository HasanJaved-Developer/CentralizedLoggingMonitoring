namespace CentralizedLoggingApi.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string Role { get; set; } = "Developer"; // Admin, Developer
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
