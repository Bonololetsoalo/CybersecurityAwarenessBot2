using System;
using CybersecurityAwarenessBot.Core;
using CybersecurityAwarenessBot.Models;
using CybersecurityAwarenessBot.Services;

namespace CybersecurityAwarenessBot
{
    public class BotEngine
    {
        private User? _currentUser;
        private readonly ResponseManager _responseManager;

        public BotEngine()
        {
            _responseManager = new ResponseManager();
        }

        public void Start()
        {
            AudioPlayer.PlayGreetingSound();
            DisplayHeader();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Enter your name to start: ");
            Console.ResetColor();

            string? rawName = Console.ReadLine();
            _currentUser = new User(rawName);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\nWelcome, {_currentUser.Name}! Ask me any cybersecurity question or type 'exit' to quit.");
            Console.ResetColor();

            ChatLoop();
        }

        private void ChatLoop()
        {
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write($"\n[{_currentUser?.Name}]: ");
                Console.ResetColor();

                string? input = Console.ReadLine();

                if (InputValidator.IsNullOrEmpty(input))
                {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine("[Bot]: Input cannot be empty. Please type a question.");
                    Console.ResetColor();
                    continue;
                }

                if (input!.Trim().Equals("exit", StringComparison.OrdinalIgnoreCase) ||
                    input.Trim().Equals("quit", StringComparison.OrdinalIgnoreCase))
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine($"\n[Bot]: Goodbye, {_currentUser?.Name}! Stay safe online!");
                    Console.ResetColor();
                    break;
                }

                string reply = _responseManager.GetResponse(input);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"[Bot]: {reply}");
                Console.ResetColor();
            }
        }

        private void DisplayHeader()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(@"
   ______     __               ____        __ 
  / ____/_  _/ /_  ___  _____ / __ )____  / /_
 / /   / / / / __ \/ _ \/ ___/ __  / __ \/ __/
/ /___/ /_/ / /_/ /  __/ /  / /_/ / /_/ / /_  
\____/\__, /_.___/\___/_/  /_____/\____/\__/  
     /____/                                   ");
            Console.WriteLine("=================================================");
            Console.WriteLine("       CYBERSECURITY AWARENESS CHATBOT           ");
            Console.WriteLine("=================================================\n");
            Console.ResetColor();
        }
    }
}