using System;
using System.Collections.Generic;
using CybersecurityAwarenessBot.Models;

namespace CybersecurityAwarenessBot.Services
{
    public class ResponseManager
    {
        private readonly User _user;
        private readonly Random _random = new Random();
        private string _lastTopic = string.Empty;

        private readonly DatabaseHelper _dbHelper = new DatabaseHelper();
        private readonly QuizManager _quizManager = new QuizManager();
        private string? _pendingTaskTitle = null;

        private readonly List<int> _usedTipIndices = new List<int>();

        private readonly Dictionary<string, List<string>> _topicTips = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
        {
            {
                "phishing", new List<string>
                {
                    "Be careful with emails asking for personal info—scammers often pretend to be trusted companies.",
                    "Always check the sender's full email address for small typos before clicking any links.",
                    "Hover your mouse over links to see the real web address before you click on them.",
                    "Don't fall for urgent messages demanding immediate money or action—scammers rely on panic."
                }
            },
            {
                "password", new List<string>
                {
                    "Use a secure Password Manager app to create and remember complex passwords for you automatically.",
                    "Make passwords long (at least 12 characters) and mix uppercase, lowercase, numbers, and symbols.",
                    "Turn on Two-Factor Authentication (2FA/MFA) so your account asks for a phone code even if someone steals your password.",
                    "Never use the same password for multiple accounts—if one site gets hacked, all your accounts become vulnerable."
                }
            },
            {
                "privacy", new List<string>
                {
                    "Check your social media privacy settings regularly so strangers can't see your personal posts.",
                    "Avoid posting sensitive information online like your home address, phone number, or full birth date.",
                    "Use a VPN when connecting to free public Wi-Fi to keep your private activity encrypted."
                }
            },
            {
                "safe browsing", new List<string>
                {
                    "Check that the web address starts with 'https://' and shows a padlock icon before typing passwords.",
                    "Never download apps or files from sketchy third-party websites.",
                    "Keep your web browser updated so it can automatically block malicious and harmful websites."
                }
            }
        };

        public ResponseManager(User user)
        {
            _user = user ?? new User("User");
        }

        public string GetResponse(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return "I'm not sure I understand. Can you try rephrasing?";

            string cleanInput = input.Trim();
            string text = cleanInput.ToLower();

            // 1. PENDING REMINDER PROMPT RESPONSE (Handles inputs like "3 days", "tomorrow", "none")
            if (_pendingTaskTitle != null)
            {
                if (text.StartsWith("add") || text.StartsWith("remind") || text.Contains("quiz") || text.Contains("what have you done"))
                {
                    _pendingTaskTitle = null;
                }
                else
                {
                    string reminderSetting = cleanInput;
                    string savedTitle = _pendingTaskTitle;
                    _pendingTaskTitle = null;

                    if (text.Equals("none", StringComparison.OrdinalIgnoreCase) || text.Equals("no", StringComparison.OrdinalIgnoreCase))
                    {
                        _dbHelper.AddTask(savedTitle, "User task", "No reminder");
                        _dbHelper.LogActivity($"Task added: '{savedTitle}' (no reminder set)");
                        return $"Task added: '{savedTitle}'. (No reminder set)";
                    }
                    else
                    {
                        _dbHelper.AddTask(savedTitle, "User task", reminderSetting);
                        _dbHelper.LogActivity($"Reminder set for '{savedTitle}' ({reminderSetting})");
                        return $"Reminder set for '{savedTitle}' ({reminderSetting}). Task saved to database!";
                    }
                }
            }

            // 2. QUIZ ACTIVE STATE CHECK
            if (_quizManager.IsQuizActive)
            {
                if (text.Equals("exit", StringComparison.OrdinalIgnoreCase) ||
                    text.Equals("quit", StringComparison.OrdinalIgnoreCase) ||
                    text.Equals("pause", StringComparison.OrdinalIgnoreCase) ||
                    text.Equals("stop", StringComparison.OrdinalIgnoreCase) ||
                    text.Equals("later", StringComparison.OrdinalIgnoreCase))
                {
                    _dbHelper.LogActivity("Quiz paused/exited by user");
                    return _quizManager.PauseQuiz() + "\n\nYou are out of the quiz. What would you like to do next?";
                }

                if (!_quizManager.IsValidQuizAnswer(input))
                {
                    string pauseMessage = _quizManager.PauseQuiz();
                    _dbHelper.LogActivity("Quiz auto-paused by non-quiz user query");
                    string actualResponse = HandleBotIntents(cleanInput, text);
                    return $"{pauseMessage}\n\n{actualResponse}";
                }

                return _quizManager.ProcessAnswer(input);
            }

            return HandleBotIntents(cleanInput, text);
        }

        private string HandleBotIntents(string cleanInput, string text)
        {
            // 3. QUIZ START INTENT
            if (text.Contains("quiz") || text.Contains("start quiz") || text.Contains("resume quiz"))
            {
                _dbHelper.LogActivity("Started Cybersecurity Quiz");
                return _quizManager.StartQuiz();
            }

            // 4. ACTIVITY LOG QUERY (Task 4 requirement - Shows all logged actions sequentially)
            if (text.Contains("what have you done") || text.Contains("show log") || text.Contains("activity log"))
            {
                List<string> logs = _dbHelper.GetRecentActivityLogs(10); // Expanded limit to show all recent actions
                if (logs == null || logs.Count == 0)
                {
                    return "No recent actions recorded yet.";
                }

                string logSummary = "Here's a summary of recent actions:\n";
                for (int i = 0; i < logs.Count; i++)
                {
                    logSummary += $"{i + 1}. {logs[i]}\n";
                }
                return logSummary.TrimEnd();
            }

            // 5. VIEW TASKS INTENT
            if (text.Equals("show tasks") || text.Equals("list tasks") || text.Equals("view tasks"))
            {
                var tasks = _dbHelper.GetAllTasks();
                if (tasks.Count == 0) return "You have no saved tasks in the database.";

                string taskList = "Here are your saved tasks:\n";
                foreach (var t in tasks)
                {
                    string status = t.IsCompleted ? "[Completed]" : "[Pending]";
                    taskList += $"- ID {t.Id}: {t.Title} ({status}) | Reminder: {t.ReminderDate}\n";
                }
                return taskList.TrimEnd();
            }

            // 6. COMPLETE TASK INTENT
            if (text.StartsWith("complete task ") || text.StartsWith("mark task "))
            {
                string idPart = text.Replace("complete task", "").Replace("mark task", "").Replace("completed", "").Trim();
                if (int.TryParse(idPart, out int taskId))
                {
                    _dbHelper.CompleteTask(taskId);
                    _dbHelper.LogActivity($"Marked task ID {taskId} as completed");
                    return $"Task ID {taskId} has been marked as completed and updated in the database!";
                }
                return "Please specify a valid task ID to complete (e.g., 'complete task 1').";
            }

            // 7. DELETE TASK INTENT
            if (text.StartsWith("delete task ") || text.StartsWith("remove task "))
            {
                string idPart = text.Replace("delete task", "").Replace("remove task", "").Trim();
                if (int.TryParse(idPart, out int taskId))
                {
                    _dbHelper.DeleteTask(taskId);
                    _dbHelper.LogActivity($"Deleted task ID {taskId}");
                    return $"Task ID {taskId} has been deleted from the database.";
                }
                return "Please specify a valid task ID to delete (e.g., 'delete task 1').";
            }

            // 8. DIRECT REMINDER INTENT
            if (text.StartsWith("remind me to "))
            {
                _pendingTaskTitle = null;
                string content = cleanInput[13..].Trim();
                string timeframe = "tomorrow";

                if (text.EndsWith(" tomorrow"))
                {
                    content = content[..^9].Trim();
                    timeframe = "tomorrow";
                }

                if (content.Length > 0)
                    content = char.ToUpper(content[0]) + content[1..];

                _dbHelper.AddTask(content, "Reminder Task", timeframe);
                _dbHelper.LogActivity($"Reminder set for '{content}' ({timeframe})");

                return $"Reminder set for '{content}' on {timeframe}.";
            }

            // 9. TASK CREATION INTENT (Logs the initial creation immediately!)
            if (text.StartsWith("add task") || text.StartsWith("add a task"))
            {
                string taskTitle = cleanInput;
                if (text.StartsWith("add task"))
                    taskTitle = cleanInput[8..].Trim();
                else if (text.StartsWith("add a task"))
                    taskTitle = cleanInput[10..].Trim();

                if (taskTitle.StartsWith("-") || taskTitle.StartsWith("to"))
                    taskTitle = taskTitle.TrimStart('-', 't', 'o').Trim();

                if (taskTitle.Length > 0)
                    taskTitle = char.ToUpper(taskTitle[0]) + taskTitle[1..];
                else
                    taskTitle = "Security Task";

                _pendingTaskTitle = taskTitle;
                _dbHelper.LogActivity($"Task added: '{taskTitle}'"); // Logs creation right away
                return $"Task added: '{taskTitle}'. Would you like to set a reminder for this task? (e.g., '3 days', 'tomorrow', or 'none')";
            }

            // 10. Memory & Recall Setting
            if (text.Contains("interested in") || text.Contains("favourite topic is") || text.Contains("favorite topic is"))
            {
                string detectedTopic = DetectTopic(text);
                if (!string.IsNullOrEmpty(detectedTopic))
                {
                    _user.FavoriteTopic = detectedTopic;
                    SetTopic(detectedTopic);
                    return $"Great! I'll remember that you're interested in {detectedTopic}. It's a crucial part of staying safe online.";
                }
            }

            // 11. Sentiment Detection: Worried / Scared / Frustrated / Curious
            if (text.Contains("worried") || text.Contains("scared") || text.Contains("hacked"))
            {
                string tip = !string.IsNullOrEmpty(_lastTopic) ? GetUniqueTipForTopic(_lastTopic) : GetUniqueTipForTopic("phishing");
                return $"Scammers can be very convincing, {_user.Name}. Don't worry! Let me share a tip to help you stay safe: {tip}";
            }

            if (text.Contains("curious") || text.Contains("excited") || text.Contains("great"))
            {
                return $"It's great that you're curious about cybersecurity, {_user.Name}! Staying informed is your best defense.";
            }

            // 12. Confusion / Reassurance
            if (text.Contains("confused") || text.Contains("don't understand") || text.Contains("unclear"))
            {
                if (!string.IsNullOrEmpty(_lastTopic))
                {
                    return $"No worries, {_user.Name}! Let's make {_lastTopic} safety simple: focus on one step at a time. Here is another simple rule: {GetUniqueTipForTopic(_lastTopic)}";
                }

                return $"Cybersecurity can feel like a lot to take in, {_user.Name}. Ask me about 'passwords', 'phishing', or 'privacy' and we will go step-by-step!";
            }

            // 13. Explicit Topic Detection
            string explicitTopic = DetectTopic(text);
            if (!string.IsNullOrEmpty(explicitTopic))
            {
                SetTopic(explicitTopic);
                string tip = GetUniqueTipForTopic(_lastTopic);

                if (!string.IsNullOrEmpty(_user.FavoriteTopic) &&
                    _user.FavoriteTopic.Equals(_lastTopic, StringComparison.OrdinalIgnoreCase))
                {
                    return $"As someone interested in {_lastTopic}, here is a key tip for you: {tip}";
                }

                return $"Here is a tip for {_lastTopic}: {tip}";
            }

            // 14. Follow-up requests
            if (text.Contains("more") || text.Contains("another tip") || text.Contains("explain"))
            {
                if (!string.IsNullOrEmpty(_lastTopic))
                {
                    return $"Here is another detail regarding {_lastTopic}: {GetUniqueTipForTopic(_lastTopic)}";
                }

                return "Which topic would you like to hear more about? You can ask about 'passwords', 'phishing', 'privacy', or 'safe browsing'.";
            }

            return "I'm not sure I understand. Can you try rephrasing? You can ask me about 'phishing', 'passwords', 'privacy', or 'safe browsing'.";
        }

        private void SetTopic(string topic)
        {
            if (!_lastTopic.Equals(topic, StringComparison.OrdinalIgnoreCase))
            {
                _lastTopic = topic;
                _usedTipIndices.Clear();
            }
        }

        private string DetectTopic(string text)
        {
            if (text.Contains("phishing") || text.Contains("phish") || text.Contains("scam") || text.Contains("email"))
                return "phishing";

            if (text.Contains("password") || text.Contains("passcode") || text.Contains("pass") || text.Contains("mfa") || text.Contains("2fa"))
                return "password";

            if (text.Contains("privacy") || text.Contains("private") || text.Contains("data"))
                return "privacy";

            if (text.Contains("browsing") || text.Contains("browersing") || text.Contains("browser") || text.Contains("browsering") || text.Contains("https"))
                return "safe browsing";

            return string.Empty;
        }

        private string GetUniqueTipForTopic(string topic)
        {
            if (_topicTips.ContainsKey(topic) && _topicTips[topic].Count > 0)
            {
                List<string> tips = _topicTips[topic];

                if (_usedTipIndices.Count >= tips.Count)
                {
                    _usedTipIndices.Clear();
                }

                int newIndex;
                do
                {
                    newIndex = _random.Next(tips.Count);
                } while (_usedTipIndices.Contains(newIndex));

                _usedTipIndices.Add(newIndex);
                return tips[newIndex];
            }

            return "Always keep your devices updated and use strong, unique passwords.";
        }
    }
}