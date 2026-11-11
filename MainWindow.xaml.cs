using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using CybersecurityAwarenessBot.Core; // Ensure this matches your AudioPlayer namespace
using CybersecurityAwarenessBot.Models;
using CybersecurityAwarenessBot.Services;

namespace CybersecurityAwarenessBot
{
    public partial class MainWindow : Window
    {
        private User currentUser;
        private ResponseManager responseManager;
        private bool isNameEntered = false;

        public MainWindow()
        {
            InitializeComponent();

            currentUser = new User("User");
            responseManager = new ResponseManager(currentUser);

            // Triggers the startup audio greeting
            AudioPlayer.PlayGreeting();

            AppendBotMessage("Welcome to the Cybersecurity Awareness Bot! Enter your name to start:");
        }

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            await ProcessUserInput();
        }

        private async void InputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                await ProcessUserInput();
            }
        }

        private async Task ProcessUserInput()
        {
            string userInput = InputTextBox.Text.Trim();
            if (string.IsNullOrEmpty(userInput)) return;

            if (!isNameEntered)
            {
                currentUser.Name = userInput;
                isNameEntered = true;

                AppendUserMessage(userInput);
                InputTextBox.Clear();

                AppendBotMessage($"Hello, {currentUser.Name}! How can I help you with cybersecurity today? " +
                    $"Type 'exit' when you are done");
                return;
            }

            AppendUserMessage(userInput);
            InputTextBox.Clear();

            if (userInput.Equals("exit", StringComparison.OrdinalIgnoreCase))
            {
                AppendBotMessage($"Goodbye, {currentUser.Name}! Stay safe online. " +
                    $"Closing chat...");

                InputTextBox.IsEnabled = false;
                if (FindName("SendButton") is Button btn) btn.IsEnabled = false;

                await Task.Delay(2000);

                this.Close();
                return;
            }

            string botResponse = responseManager.GetResponse(userInput);
            AppendBotMessage(botResponse);
        }

        private void AppendUserMessage(string message)
        {
            Paragraph p = new Paragraph();
            Brush userBrush = new SolidColorBrush(Color.FromRgb(37, 211, 102)); // WhatsApp Green

            Run tagRun = new Run($"{currentUser.Name ?? "User"}: ") { Foreground = userBrush, FontWeight = FontWeights.Bold };
            Run textRun = new Run(message) { Foreground = userBrush };

            p.Inlines.Add(tagRun);
            p.Inlines.Add(textRun);
            ChatDisplay.Document.Blocks.Add(p);
            ChatDisplay.ScrollToEnd();
        }

        private void AppendBotMessage(string message)
        {
            Paragraph p = new Paragraph();
            Brush botBrush = new SolidColorBrush(Color.FromRgb(255, 215, 0)); // Gold

            Run tagRun = new Run("Bot: ") { Foreground = botBrush, FontWeight = FontWeights.Bold };
            Run textRun = new Run(message) { Foreground = botBrush };

            p.Inlines.Add(tagRun);
            p.Inlines.Add(textRun);
            ChatDisplay.Document.Blocks.Add(p);
            ChatDisplay.ScrollToEnd();
        }
    }
}