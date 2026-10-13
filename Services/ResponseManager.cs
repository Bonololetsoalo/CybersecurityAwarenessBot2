using CybersecurityAwarenessBot.Models;
using System.Text.RegularExpressions;

namespace CybersecurityAwarenessBot.Services
{
    public delegate string SentimentHandler(string sentiment);

    public class ResponseManager
    {
        private readonly User _currentUser;
        private readonly Random _random;

        private readonly Dictionary<string, List<string>> _randomTopicTips;
        private readonly Dictionary<string, string> _detailedExplanations;

        public ResponseManager(User user)
        {
            _currentUser = user;
            _random = new Random();

            _randomTopicTips = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
            {
                {
                    "phishing", new List<string>
                    {
                        "Phishing is a cyber attack where scammers impersonate trusted organizations via email or message to steal sensitive data.",
                        "Always inspect the sender's email address carefully for subtle domain misspellings.",
                        "Never click suspicious links or download unsolicited attachments from unknown senders."
                    }
                },
                {
                    "password", new List<string>
                    {
                        "Use passwords at least 12 characters long with a mix of uppercase, lowercase, numbers, and symbols. Never reuse passwords!",
                        "Make sure to use strong, unique passwords for each account. Avoid using personal details in your passwords.",
                        "Consider using a secure password manager to store and generate complex credentials."
                    }
                },
                {
                    "passwords", new List<string>
                    {
                        "Use passwords at least 12 characters long with a mix of uppercase, lowercase, numbers, and symbols. Never reuse passwords!",
                        "Make sure to use strong, unique passwords for each account. Avoid using personal details in your passwords.",
                        "Consider using a secure password manager to store and generate complex credentials."
                    }
                },
                {
                    "safe browsing", new List<string>
                    {
                        "Ensure websites use HTTPS (look for the padlock icon), keep your browser updated, and avoid downloading attachments from unknown sources.",
                        "Regularly audit the privacy settings on your social media accounts and mobile apps.",
                        "Avoid using public Wi-Fi networks for financial transactions without a trusted VPN."
                    }
                },
                {
                    "browsing", new List<string>
                    {
                        "Ensure websites use HTTPS (look for the padlock icon), keep your browser updated, and avoid downloading attachments from unknown sources.",
                        "Regularly audit the privacy settings on your social media accounts and mobile apps.",
                        "Avoid using public Wi-Fi networks for financial transactions without a trusted VPN."
                    }
                },
                {
                    "purpose", new List<string>
                    {
                        "I am a Cybersecurity Awareness Chatbot designed to help you learn best practices for staying safe online!"
                    }
                }
            };

            _detailedExplanations = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "phishing", "Phishing relies on social engineering. Scammers manipulate fear or urgency to trick you into giving credentials or money." },
                { "password", "Strong passwords and Multi-Factor Authentication (MFA) make it substantially harder for attackers to compromise your accounts." },
                { "passwords", "Strong passwords and Multi-Factor Authentication (MFA) make it substantially harder for attackers to compromise your accounts." },
                { "safe browsing", "Safe browsing practices block malicious scripts, protect against drive-by downloads, and secure your session data." },
                { "browsing", "Safe browsing practices block malicious scripts, protect against drive-by downloads, and secure your session data." },
                { "purpose", "My purpose is to guide users through fundamental online security practices and reduce security vulnerabilities." }
            };
        }

        public string GetResponse(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return "Please enter a topic or question! You can ask about passwords, phishing, safe browsing, or my purpose.";
            }

            string cleanInput = input.Trim().ToLower();

            string sentimentPrefix = EvaluateSentiment(cleanInput);

            if (cleanInput.Contains("interested in") || cleanInput.Contains("favorite topic"))
            {
                foreach (var topic in _randomTopicTips.Keys)
                {
                    if (cleanInput.Contains(topic))
                    {
                        _currentUser.FavoriteTopic = topic;
                        _currentUser.LastTopic = topic;
                        return $"{sentimentPrefix}Great! I'll remember that you're interested in {topic}. It's vital for online defense.";
                    }
                }
            }

            if (cleanInput.Contains("tell me more") || cleanInput.Contains("explain more") || cleanInput.Contains("more details"))
            {
                if (!string.IsNullOrEmpty(_currentUser.LastTopic) && _detailedExplanations.ContainsKey(_currentUser.LastTopic))
                {
                    return $"{sentimentPrefix}Here is more details on **{_currentUser.LastTopic.ToUpper()}**: {_detailedExplanations[_currentUser.LastTopic]}";
                }
            }

            if (cleanInput.Contains("another tip") || cleanInput.Contains("more tips"))
            {
                if (!string.IsNullOrEmpty(_currentUser.LastTopic) && _randomTopicTips.ContainsKey(_currentUser.LastTopic))
                {
                    return $"{sentimentPrefix}Here is another tip for **{_currentUser.LastTopic}**: {GetRandomTip(_currentUser.LastTopic)}";
                }
            }

            foreach (var topic in _randomTopicTips.Keys)
            {
                if (Regex.IsMatch(cleanInput, $@"\b{Regex.Escape(topic)}\b", RegexOptions.IgnoreCase))
                {
                    _currentUser.LastTopic = topic;

                    string recallNote = "";
                    if (!string.IsNullOrEmpty(_currentUser.FavoriteTopic) &&
                        _currentUser.FavoriteTopic.Equals(topic, StringComparison.OrdinalIgnoreCase))
                    {
                        recallNote = $"As someone interested in {topic}, ";
                    }

                    return $"{sentimentPrefix}{recallNote}{GetRandomTip(topic)}";
                }
            }

            if (cleanInput.Contains("hello") || cleanInput.Equals("hi"))
            {
                return $"{sentimentPrefix}Hello, {_currentUser.Name}! Ready to help you learn online safety. What would you like to know?";
            }
            if (cleanInput.Contains("how are you"))
            {
                return $"{sentimentPrefix}I am doing great, thanks for asking! Ready to answer your cybersecurity questions.";
            }

            if (cleanInput.Contains("what do you remember") || cleanInput.Contains("my details") || cleanInput.Contains("who am i"))
            {
                string fav = string.IsNullOrEmpty(_currentUser.FavoriteTopic) ? "not set yet" : _currentUser.FavoriteTopic;
                return $"Your name is {_currentUser.Name}, and your recorded favorite topic is '{fav}'.";
            }

            return $"{sentimentPrefix}I am not sure about that topic yet. You can ask me about passwords, phishing, safe browsing, or my purpose!";
        }

        private string GetRandomTip(string topic)
        {
            var tips = _randomTopicTips[topic];
            int index = _random.Next(tips.Count);
            return tips[index];
        }

        private string EvaluateSentiment(string input)
        {
            SentimentHandler handler = (sentiment) =>
            {
                return sentiment switch
                {
                    "worried" => "It's completely understandable to feel that way. Scammers can be very convincing. Let me share some tips to help you stay safe.\n\n",
                    "frustrated" => "I understand cybersecurity can feel overwhelming. Let's break it down into simple steps.\n\n",
                    "curious" => "Curiosity is the best way to stay ahead of online threats! Here is what you should know:\n\n",
                    _ => ""
                };
            };

            if (input.Contains("worried") || input.Contains("scared") || input.Contains("afraid"))
            {
                _currentUser.DetectedSentiment = "worried";
                return handler("worried");
            }
            if (input.Contains("frustrated") || input.Contains("confused") || input.Contains("hard"))
            {
                _currentUser.DetectedSentiment = "frustrated";
                return handler("frustrated");
            }
            if (input.Contains("curious") || input.Contains("wondering") || input.Contains("want to learn"))
            {
                _currentUser.DetectedSentiment = "curious";
                return handler("curious");
            }

            return "";
        }
    }
}