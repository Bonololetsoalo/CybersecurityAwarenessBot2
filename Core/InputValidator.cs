namespace CybersecurityAwarenessBot.Core
{
    public static class InputValidator
    {
        public static bool IsNullOrEmpty(string? input)
        {
            return string.IsNullOrWhiteSpace(input);
        }
    }
}