# Cybersecurity Awareness Bot (C# WPF Application)

A desktop cybersecurity awareness assistant built with **C# and WPF (.NET)**, backed by an **SQLite** database (`cyberbot.db`). The application combines multimedia startup elements, Natural Language Processing (NLP) keyword recognition, sentiment detection, conversational memory, task/reminder automation, and an interactive radio-button quiz with pause/resume state retention.

---

## Video Demonstration

Watch the complete walkthrough covering the backend architecture (`DatabaseHelper.cs`, `User.cs`, `QuizManager.cs`, `ResponseManager.cs`) and live demonstration of Parts 1, 2, and 3:

[Cybersecurity Awareness Bot Demo] Youtube: https://youtu.be/oFZ9PHUzsEc?si=9zM3RQn1cHAnHYRl

---

## Features

### Part 1: Multimedia & User Interface
* **Automated Voice Greeting:** Plays a recorded `.wav` audio greeting upon launch using `System.Media.SoundPlayer`.
* **Custom ASCII Banner:** Displays a styled ASCII art header inside a high-contrast dark and cyan WPF interface.
* **User Personalization:** Prompts for the user's name on startup to personalize greetings and session interactions.
* **Graceful Error Handling:** Catches unrecognized commands and empty inputs with helpful fallback prompts.

### Part 2: NLP, Sentiment & Memory
* **Keyword Recognition:** Parses user input to distinguish between definitions (e.g., *"What is phishing?"*) and randomized practical tips (e.g., *"Give me a phishing tip"*), covering phishing, password hygiene, and safe browsing.
* **Sentiment Detection:** Recognizes expressions of worry or frustration and responds with empathetic, supportive guidance.
* **Conversational Memory:** Records user-stated topics of interest and recalls them later in the session.

### Part 3: Task Automation, Quiz & SQLite Persistence
* **Flexible Task & Reminder Parsing:** Supports natural task-addition variations (e.g., `"add a task two-factor authentication"`, `"add task - review privacy settings"`) and captures follow-up reminder timeframes (`"tomorrow"`, `"3 days"`).
* **SQLite Persistence (`cyberbot.db`):** Stores tasks (`Tasks` table) and system actions (`ActivityLog` table) locally so data persists across application restarts.
* **Interactive Radio-Button Quiz:** Launches a separate quiz window with multiple-choice questions, real-time scoring, and full **Pause & Resume** state retention.
* **Live Activity Audit Trail:** Tracks all queries, task updates, and quiz events, retrievable via `"show activity log"` or `"what have you done for me"`.

---

### Prerequisites
* **Visual Studio 2022** (with the *.NET desktop development* workload installed)
* **.NET 6.0 / 8.0 SDK** (Windows)
* **NuGet Package:** `Microsoft.Data.Sqlite`

### Installation & Running
1. Clone the repository:
   ```bash
      git clone [https://github.com/Bonololetsoalo/CybersecurityAwarenessBot2]
