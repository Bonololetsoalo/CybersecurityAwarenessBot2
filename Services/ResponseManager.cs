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
                // Greetings & Chit-chat
                { "hello", "Hello! I am doing well and ready to help you navigate online safety. What would you like to know today?" },
                { "hi", "Hi there! How can I help you stay safe online today?" },
                { "how are you", "I am doing great, thanks for asking! Ready to answer your cybersecurity questions." },

                // Core Topics & Plural Variations
                { "phishing", "Phishing is a cyber attack where scammers impersonate trusted organizations via email or message to steal sensitive data." },
                { "password", "Use passwords at least 12 characters long with a mix of uppercase, lowercase, numbers, and symbols. Never reuse passwords!" },
                { "passwords", "Use passwords at least 12 characters long with a mix of uppercase, lowercase, numbers, and symbols. Never reuse passwords!" },
                { "safe browsing", "Ensure websites use HTTPS (look for the padlock icon), keep your browser updated, and avoid downloading attachments from unknown sources." },
                { "browsing", "Ensure websites use HTTPS (look for the padlock icon), keep your browser updated, and avoid downloading attachments from unknown sources." },
                { "purpose", "I am a Cybersecurity Awareness Chatbot designed to help you learn best practices for staying safe online!" },

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

            foreach (var key in _responses.Keys)
            {
                if (input.Contains(key, StringComparison.OrdinalIgnoreCase))
                {
                    return _responses[key];
                }
            }

            // Fallback response for unrecognized topics
            return "I am not sure about that topic yet. You can ask me about passwords, phishing, safe browsing, or my purpose!";
        }
    }
}