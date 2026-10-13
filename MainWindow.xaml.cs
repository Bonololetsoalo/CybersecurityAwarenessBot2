using System.Windows;
using System.Windows.Input;
using CybersecurityAwarenessBot.Core;
using CybersecurityAwarenessBot.Models;
using CybersecurityAwarenessBot.Services;

namespace CybersecurityAwarenessBot
{
    public partial class MainWindow : Window
    {
        private User _user;
        private ResponseManager _responseManager;
        private bool _isNameSet = false;

        public MainWindow()
        {
            InitializeComponent();
            _user = new User("Guest User");
            _responseManager = new ResponseManager(_user);

            // Audio greeting trigger
            AudioPlayer.PlayGreetingSound();

            AppendBotMessage("Bot: Welcome to the Cybersecurity Awareness Bot! Enter your name to start:");
        }

        private void BtnSend_Click(object sender, RoutedEventArgs e)
        {
            ProcessInput();
        }

        private void TxtUserInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ProcessInput();
            }
        }

        private void ProcessInput()
        {
            string input = TxtUserInput.Text.Trim();
            if (string.IsNullOrEmpty(input)) return;

            AppendUserMessage($"You: {input}");
            TxtUserInput.Clear();

            if (!_isNameSet)
            {
                _user.Name = input;
                _isNameSet = true;
                AppendBotMessage($"Bot: Welcome, {_user.Name}! Ask me any cybersecurity question about passwords, phishing, safe browsing, or my purpose. Type 'exit' when you are done.");
                return;
            }

            if (input.Equals("exit", System.StringComparison.OrdinalIgnoreCase) ||
                input.Equals("quit", System.StringComparison.OrdinalIgnoreCase))
            {
                AppendBotMessage($"Bot: Goodbye, {_user.Name}! Stay safe online!");
                return;
            }

            string response = _responseManager.GetResponse(input);
            AppendBotMessage($"Bot: {response}");
        }

        private void AppendUserMessage(string message)
        {
            TxtChatOutput.Text += $"{message}\n\n";
            ChatScrollViewer.ScrollToBottom();
        }

        private void AppendBotMessage(string message)
        {
            TxtChatOutput.Text += $"{message}\n\n";
            ChatScrollViewer.ScrollToBottom();
        }
    }
}