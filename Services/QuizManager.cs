using System.Collections.Generic;

namespace CybersecurityAwarenessBot.Services
{
    public class QuizQuestion
    {
        public string QuestionText { get; set; } = string.Empty;
        public List<string> Options { get; set; } = [];
        public string CorrectAnswer { get; set; } = string.Empty;
        public string Explanation { get; set; } = string.Empty;
    }

    public class QuizManager
    {
        public bool IsQuizActive { get; private set; } = false;
        private int _currentQuestionIndex = 0;
        private int _score = 0;

        private readonly List<QuizQuestion> _questions =
        [
            new QuizQuestion
            {
                QuestionText = "Which of the following is considered a strong, secure password?",
                Options = ["A) 12345678", "B) password123", "C) P@ssw0rd!", "D) X#9mp$2vLq8!a"],
                CorrectAnswer = "D",
                Explanation = "Complex passwords combining uppercase, lowercase, numbers, and symbols are significantly harder to crack."
            },
            new QuizQuestion
            {
                QuestionText = "What should you do if you receive an unexpected email asking for your password?",
                Options = ["A) Reply with your password", "B) Delete the email", "C) Report the email as phishing", "D) Ignore it"],
                CorrectAnswer = "C",
                Explanation = "Reporting phishing emails helps security teams block malicious senders and protects others."
            },
            new QuizQuestion
            {
                QuestionText = "True or False: Public Wi-Fi networks are fully encrypted and safe for logging into personal bank accounts.",
                Options = ["A) True", "B) False"],
                CorrectAnswer = "B",
                Explanation = "False. Public Wi-Fi is often unsecured, allowing attackers to intercept unencrypted traffic."
            },
            new QuizQuestion
            {
                QuestionText = "What is 'Social Engineering' in cybersecurity?",
                Options = ["A) Fixing computer bugs", "B) Manipulating people into giving away confidential information", "C) Social media marketing", "D) Designing user interfaces"],
                CorrectAnswer = "B",
                Explanation = "Social engineering exploits human psychology rather than technical hacking vectors."
            },
            new QuizQuestion
            {
                QuestionText = "What does Two-Factor Authentication (2FA) require?",
                Options = ["A) Two different passwords", "B) Two separate user accounts", "C) Something you know and something you have", "D) Logging in twice a day"],
                CorrectAnswer = "C",
                Explanation = "2FA adds an extra layer of security by requiring two distinct forms of verification."
            },
            new QuizQuestion
            {
                QuestionText = "True or False: Clicking on links in unsolicited emails from unknown senders is a safe practice.",
                Options = ["A) True", "B) False"],
                CorrectAnswer = "B",
                Explanation = "False. Unsolicited links often lead to credential-harvesting or malware-downloading sites."
            },
            new QuizQuestion
            {
                QuestionText = "Which protocol is safer for browsing web pages securely?",
                Options = ["A) HTTP", "B) FTP", "C) HTTPS", "D) Telnet"],
                CorrectAnswer = "C",
                Explanation = "HTTPS encrypts data transferred between your browser and the web server."
            },
            new QuizQuestion
            {
                QuestionText = "What is ransomware?",
                Options = ["A) Free antivirus software", "B) Malware that encrypts your files and demands payment for decryption", "C) A secure backup tool", "D) A type of firewall"],
                CorrectAnswer = "B",
                Explanation = "Ransomware locks users out of their data until a ransom is paid to the attackers."
            },
            new QuizQuestion
            {
                QuestionText = "True or False: Reusing the same strong password across multiple websites is recommended.",
                Options = ["A) True", "B) False"],
                CorrectAnswer = "B",
                Explanation = "False. If one site is breached, attackers gain access to all your accounts using that password."
            },
            new QuizQuestion
            {
                QuestionText = "What is the primary purpose of a firewall?",
                Options = ["A) To cool down the computer processor", "B) To monitor and control incoming and outgoing network traffic", "C) To speed up internet downloads", "D) To store encrypted backups"],
                CorrectAnswer = "B",
                Explanation = "Firewalls act as a security barrier between trusted internal networks and untrusted external networks."
            }
        ];

        public string StartQuiz()
        {
            IsQuizActive = true;
            _currentQuestionIndex = 0;
            _score = 0;
            return $"Starting Cybersecurity Quiz! ({_questions.Count} Questions total).\n" +
                   "*(Type 'exit', 'pause', or simply ask any other question to leave the quiz at any time)*\n\n" +
                   GetCurrentQuestionText();
        }

        public string PauseQuiz()
        {
            IsQuizActive = false;
            return $"[Quiz Paused at Question {_currentQuestionIndex + 1} of {_questions.Count}. Current Score: {_score}/{_questions.Count}]";
        }

        public bool IsValidQuizAnswer(string answer)
        {
            string clean = answer.Trim().ToUpper();
            if (clean == "A" || clean == "B" || clean == "C" || clean == "D" || clean == "TRUE" || clean == "FALSE")
            {
                return true;
            }
            return false;
        }

        public string ProcessAnswer(string answer)
        {
            string cleanAns = answer.Trim().ToUpper();
            if (cleanAns.Length > 0) cleanAns = cleanAns[0].ToString();

            var currentQ = _questions[_currentQuestionIndex];
            bool isCorrect = cleanAns == currentQ.CorrectAnswer;

            string feedback = isCorrect
                ? $"**Correct!** {currentQ.Explanation}\n\n"
                : $"**Incorrect.** The correct answer was {currentQ.CorrectAnswer}. {currentQ.Explanation}\n\n";

            if (isCorrect) _score++;
            _currentQuestionIndex++;

            if (_currentQuestionIndex >= _questions.Count)
            {
                IsQuizActive = false;
                string remark = _score >= 8 ? "Great job! You're a cybersecurity pro!" : "Keep learning to stay safe online!";
                return feedback + $"**Quiz Completed!** Final Score: {_score}/{_questions.Count}. {remark}";
            }

            return feedback + GetCurrentQuestionText();
        }

        private string GetCurrentQuestionText()
        {
            var q = _questions[_currentQuestionIndex];
            return $"**Question {_currentQuestionIndex + 1} of {_questions.Count}:**\n{q.QuestionText}\n{string.Join("\n", q.Options)}\n\n*(Type A, B, C, or D)*";
        }
    }
}