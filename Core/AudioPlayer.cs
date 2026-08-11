using System;
using System.IO;
using System.Media;
using System.Runtime.InteropServices;

namespace CybersecurityAwarenessBot.Core
{
    public static class AudioPlayer
    {
        public static void PlayGreetingSound()
        {
            try
            {
                // Only attempt sound playback if running on Windows
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    string soundPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "welcome.wav.wav ");
                    if (File.Exists(soundPath))
                    {
                        using (SoundPlayer player = new SoundPlayer(soundPath))
                        {
                            player.Play();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Audio Warning]: Could not play greeting audio. ({ex.Message})");
            }
        }
    }
}