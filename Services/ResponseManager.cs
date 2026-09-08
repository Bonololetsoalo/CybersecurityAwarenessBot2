using System;
using System.Collections.Generic;

namespace CybersecurityAwarenessBot.Services
{
    public class ResponseManager
    {
        private readonly Dictionary<string, string> _responses;

        public ResponseManager()
        {
            _responses = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                // Core Topics (Placed FIRST so topics take priority over generic greetings)
                { "phishing", "Phishing is a cyber attack where scammers impersonate trusted organizations via email or message to steal sensitive data." },
                { "passwords", "Use passwords at least 12 characters long with a mix of uppercase, lowercase, numbers, and symbols. Never reuse passwords!" },
                { "password", "Use passwords at least 12 characters long with a mix of uppercase, lowercase, numbers and symbols. Never reuse passwords!" },
                { "safe browsing", "Ensure websites use HTTPS (look for the padlock icon), keep your browser updated and avoid downloading attachments from unknown sources." },
                { "browsing", "Ensure websites use HTTPS (look for the padlock icon), keep your browser updated, and avoid downloading attachments from unknown sources." },
                { "purpose", "I am a Cybersecurity Awareness Chatbot designed to help you learn best practices for staying safe online!" },

                // Chit-chat & Greetings (Short words like 'hi' or 'hello' placed after specific topics)
                { "hello", "Hello! I am doing well and ready to help you navigate online safety. What would you like to know today?" },
                { "how are you", "I am doing great, thanks for asking! Ready to answer your cybersecurity questions." },
                { "hi", "Hi there! How can I help you stay safe online today?" },

                // Exit options
                { "exit", "Goodbye! Stay safe online!" },
                { "quit", "Goodbye! Stay safe online!" }
            };
        }

        public string GetResponse(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return "Please enter a topic or question! You can ask about passwords, phishing, safe browsing, or my purpose.";
            }

            // Clean up the user input into individual words
            string cleanInput = input.Trim();

            // 1. First Check: Check for exact topic keyword matches in the dictionary
            foreach (var key in _responses.Keys)
            {
                // Check for whole word match to prevent "hi" matching inside "phishing"
                if (System.Text.RegularExpressions.Regex.IsMatch(cleanInput, $@"\b{System.Text.RegularExpressions.Regex.Escape(key)}\b", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                {
                    return _responses[key];
                }
            }

            // Fallback response for unrecognized topics
            return "I am not sure about that topic yet. You can ask me about passwords, phishing, safe browsing, or my purpose!";
        }
    }
}