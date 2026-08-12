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
                { "phishing", "Phishing is a cyber attack where scammers impersonate trusted organizations via email or message to steal sensitive data." },
                { "password", "Use passwords at least 12 characters long with a mix of uppercase, lowercase, numbers, and symbols. Never reuse passwords!" },
                { "safe browsing", "Ensure websites use HTTPS (look for the padlock icon), keep your browser updated, and avoid downloading attachments from unknown sources." },
                { "purpose", "I am a Cybersecurity Awareness Chatbot designed to help you learn best practices for staying safe online!" }
            };
        }

        public string GetResponse(string input)
        {
            foreach (var key in _responses.Keys)
            {
                if (input.Contains(key, StringComparison.OrdinalIgnoreCase))
                {
                    return _responses[key];
                }
            }
            return "I am not sure about that topic yet. You can ask me about passwords, phishing, safe browsing, or my purpose!";
        }
    }
}