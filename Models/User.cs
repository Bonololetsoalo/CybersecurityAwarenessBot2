namespace CybersecurityAwarenessBot.Models
{
    public class User
    {
        public string Name { get; set; }

        public User(string? name)
        {
            Name = string.IsNullOrWhiteSpace(name) ? "Guest User" : name;
        }
    }
}
