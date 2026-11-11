using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using CybersecurityAwarenessBot.Models;

namespace CybersecurityAwarenessBot.Services
{
    public class DatabaseHelper
    {
        private readonly string _connectionString = "Data Source=cybersecurity.db";

        public DatabaseHelper()
        {
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            // Self-healing check: Drop tables if they don't have the correct Id schema to prevent crashes
            string checkTable = "PRAGMA table_info(ActivityLogs);";
            bool hasIdColumn = false;
            using (var checkCmd = new SqliteCommand(checkTable, connection))
            {
                using var reader = checkCmd.ExecuteReader();
                while (reader.Read())
                {
                    if (reader["name"].ToString() == "Id")
                    {
                        hasIdColumn = true;
                        break;
                    }
                }
            }

            if (!hasIdColumn)
            {
                string dropOld = "DROP TABLE IF EXISTS ActivityLogs; DROP TABLE IF EXISTS Tasks;";
                using var dropCmd = new SqliteCommand(dropOld, connection);
                dropCmd.ExecuteNonQuery();
            }

            string createTasksTable = @"
                CREATE TABLE IF NOT EXISTS Tasks (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Title TEXT NOT NULL,
                    Description TEXT,
                    ReminderDate TEXT,
                    IsCompleted INTEGER DEFAULT 0
                );";

            using (var command = new SqliteCommand(createTasksTable, connection))
            {
                command.ExecuteNonQuery();
            }

            string createLogsTable = @"
                CREATE TABLE IF NOT EXISTS ActivityLogs (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    ActionText TEXT NOT NULL,
                    Timestamp DATETIME DEFAULT CURRENT_TIMESTAMP
                );";

            using (var command = new SqliteCommand(createLogsTable, connection))
            {
                command.ExecuteNonQuery();
            }
        }

        public void AddTask(string title, string description, string reminderDate)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            string query = "INSERT INTO Tasks (Title, Description, ReminderDate, IsCompleted) VALUES (@title, @description, @reminder, 0);";
            using var command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@title", title);
            command.Parameters.AddWithValue("@description", description);
            command.Parameters.AddWithValue("@reminder", reminderDate);
            command.ExecuteNonQuery();
        }

        public List<TaskItem> GetAllTasks()
        {
            var tasks = new List<TaskItem>();
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            string query = "SELECT Id, Title, Description, ReminderDate, IsCompleted FROM Tasks;";
            using var command = new SqliteCommand(query, connection);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                tasks.Add(new TaskItem
                {
                    Id = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    Description = reader.IsDBNull(2) ? "" : reader.GetString(2),
                    ReminderDate = reader.IsDBNull(3) ? "" : reader.GetString(3),
                    IsCompleted = reader.GetInt32(4) == 1
                });
            }
            return tasks;
        }

        public void CompleteTask(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            string query = "UPDATE Tasks SET IsCompleted = 1 WHERE Id = @id;";
            using var command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@id", id);
            command.ExecuteNonQuery();
        }

        public void DeleteTask(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            string query = "DELETE FROM Tasks WHERE Id = @id;";
            using var command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@id", id);
            command.ExecuteNonQuery();
        }

        public void LogActivity(string actionText)
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            string query = "INSERT INTO ActivityLogs (ActionText) VALUES (@action);";
            using var command = new SqliteCommand(query, connection);
            command.Parameters.AddWithValue("@action", actionText);
            command.ExecuteNonQuery();
        }

        public List<string> GetRecentActivityLogs(int count = 10)
        {
            var logs = new List<string>();
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();
            string query = $"SELECT ActionText FROM (SELECT ActionText, Id FROM ActivityLogs ORDER BY Id DESC LIMIT {count}) ORDER BY Id ASC;";
            using var command = new SqliteCommand(query, connection);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                logs.Add(reader.GetString(0));
            }
            return logs;
        }
    }
}