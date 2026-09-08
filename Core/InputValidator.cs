namespace CybersecurityAwarenessBot.Core

{

    // A static class responsible for validating user input

    public static class InputValidator

    {

        // Checks if the provided string is null, empty, or consists only of whitespace

        // True if the string is null, empty, or whitespace; otherwise, false.

        public static bool IsNullOrEmpty(string? input)

        {

            // Uses built-in .NET method to check for null, empty, or whitespace-only strings



            return string.IsNullOrWhiteSpace(input);

        }

    }

}