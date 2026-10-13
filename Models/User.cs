namespace CybersecurityAwarenessBot.Models
{
    public class User
    {
        public string Name { get; set; }
        public string? FavoriteTopic { get; set; }
        public string? LastTopic { get; set; }
        public string? DetectedSentiment { get; set; }

        public User(string? name)
        {
            Name = string.IsNullOrWhiteSpace(name) ? "Guest User" : name.Trim();
        }
    }
}