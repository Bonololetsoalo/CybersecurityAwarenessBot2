using System;
using System.IO;
using System.Media;

namespace CybersecurityAwarenessBot.Core // (Match your actual namespace)
{
    public static class AudioPlayer
    {
        public static void PlayGreeting()
        {
            try
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "welcome.wav.wav");
                if (File.Exists(path))
                {
                    SoundPlayer player = new SoundPlayer(path);
                    player.Play();
                }
            }
            catch (Exception)
            {
                // Prevent crashes if audio fails
            }
        }
    }
}