namespace CybersecurityAwarenessBot.Models
{
    public class User
    {
        public string Name { get; set; }
        public string FavoriteTopic { get; set; }

        public User(string name)
        {
            Name = name;
            FavoriteTopic = string.Empty;
        }
    }
}